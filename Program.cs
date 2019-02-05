using AutoMapper;
using Audit.Core;
using Audit.Core.Providers;
using Audit.EntityFramework;
using AutoMapper.QueryableExtensions;
using EfCore.Data;
using EFCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Serilog;
using Serilog.Events;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace EFCore
{
    public class Program
    {

        public static async Task Main(string[] args)
        {
         

            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

            var host = new HostBuilder()
                .UseConsoleLifetime()
                .UseSerilog()
                .ConfigureServices(services => {
                    services
                        .AddDbContext<MyAppContext>(options => {
                          //  options.UseLoggerFactory(MyLoggerFactory);
                            options.UseSqlite(Constants.ConnectionString, sqliteOptions => {
                                sqliteOptions.MigrationsAssembly(typeof(MyAppContext).GetTypeInfo().Assembly.GetName().Name);
                            });
                        });

            AutoMapper.Mapper.Reset();

            AutoMapper.Mapper.Initialize(cfg => {

                cfg.ForAllMaps((typeMap, expression) => {

                    // We should never allow saving an IAuditable thing to an IEntity. This may lead to properties like `UpdatedOn` being edited!
                    // IEntity's that are also IAuditable will have their auditable properties saved by overrides on the DbContext.
                    // Additional Note: Line items are not IEntity. But in general, they are navigation properties and not directly mapped, usually they get mapped by Id and new instances are created.
                    if (typeMap.SourceType.GetInterfaces().Contains(typeof(IAuditable)) && typeMap.DestinationType.GetInterfaces().Contains(typeof(IEntity)))
                    {
                        throw new AutoMapperMappingException($"Source type {typeMap.SourceType.Name} and dest type {typeMap.DestinationType.Name} implements {nameof(IAuditable)}. " +
                            $"Ensure you are not trying to map an auditable VM to an IEntity");
                    }

                    // Ignore any IAuditable properties from being mapped to an IEntity 9ef entity).
                    // In the tests when we call AssertConfigurationIsValid against the profiles, AutoMapper will complain because the EF entity (which is not always an IEntity) has auditable properties (like UpdatedOn)
                    // This is expected. Because we do not want to be mapping ANYTHING to those audit properties
                    if (typeMap.DestinationType.GetInterfaces().Contains(typeof(IEntity)))
                    {
                        expression
                            .ForMember(nameof(IAuditable.CreatedByDisplayName), opts => opts.Ignore())
                            .ForMember(nameof(IAuditable.CreatedByWUPeopleId), opts => opts.Ignore())
                            .ForMember(nameof(IAuditable.UpdatedByDisplayName), opts => opts.Ignore())
                            .ForMember(nameof(IAuditable.UpdatedByWUPeopleId), opts => opts.Ignore())
                            .ForMember(nameof(IAuditable.CreatedOnUtc), opts => opts.Ignore())
                            .ForMember(nameof(IAuditable.UpdatedOnUtc), opts => opts.Ignore());
                    }

                });

                cfg.CreateMap<CatUpdateViewModel, Cat>()
                    .ForMember(dest => dest.CatBreedLine,
                        opts => opts.MapFrom(src => src.CatBreedIds.Select(id => new CatBreedLine()
                        {
                            CatId = id,
                            CatBreedId = src.Id
                        })));

            });

            // Dont create audit json files by default.
            Audit.Core.Configuration.DataProvider = new NullDataProvider();

            Audit.EntityFramework.Configuration.Setup()
                .ForContext<MyAppContext>(config => {
                    config
                    .AuditEventType("{context}:{database}")
                    .IncludeEntityObjects();
                });

            Audit.Core.Configuration.AddCustomAction(ActionType.OnScopeCreated, scope =>
            {                
                scope.SetCustomField("IdentityId",  "Anonymous");
                scope.SetCustomField("IdentityDisplayName", "Anonymous");
                scope.SetCustomField("CorrelationId", Guid.NewGuid().ToString());
            });

            Audit.Core.Configuration.Setup()
                .UseEntityFramework(x =>
                    x.AuditTypeMapper(t => typeof(GenericAudit))

                        // auditEvent https://github.com/thepirat000/Audit.NET/blob/master/src/Audit.NET/AuditEvent.cs (IAuditOutput)
                        //      Can be casted to a https://github.com/thepirat000/Audit.NET/blob/master/src/Audit.EntityFramework/EntityFrameworkEvent.cs (IAuditOutput)
                        // eventEntry https://github.com/thepirat000/Audit.NET/blob/master/src/Audit.EntityFramework/EventEntry.cs (IAuditOutput)
                        //      This is our entity
                        // auditEntity is the actual IAudit entity
                        .AuditEntityAction<IGenericAudit>((auditEvent, eventEntry, auditEntity) =>
                        {
                            var entityFrameworkEvent = auditEvent.GetEntityFrameworkEvent();

                            var auditableEventEntry = (IAuditable)eventEntry.Entity;

                            auditEntity.Action = eventEntry.Action;

                            Console.WriteLine(eventEntry.ToJson());

                            auditEntity.AuditData = eventEntry.ToJson();
                            auditEntity.PrimaryKey = string.Join(',', eventEntry.PrimaryKey.Select(k => k.Value.ToString()));                            
                            auditEntity.AuditDateUtc = auditableEventEntry.UpdatedOnUtc; // Preserve the updated on datetime
                            auditEntity.EntityType = eventEntry.EntityType.Name;

                            auditEntity.AuditIdentity = auditEvent.CustomFields["IdentityId"].ToString();
                            auditEntity.AuditIdentityDisplayName = auditEvent.CustomFields["IdentityDisplayName"].ToString();
                            auditEntity.CorrelationId = auditEvent.CustomFields["CorrelationId"].ToString();
                            auditEntity.MSDuration = auditEvent.Duration;

                            auditEntity.NumObjectsEffected = entityFrameworkEvent.Result;
                            auditEntity.Success = entityFrameworkEvent.Success;
                            auditEntity.ErrorMessage = entityFrameworkEvent.ErrorMessage;

                        }));

                })
                .Build();

            using (host)
            {
                await host.StartAsync();

                var _logger = host.Services.GetService<ILogger<Program>>();

                using (var context = host.Services.GetService<MyAppContext>())
                {

                    // Delete the SQLite database
                    context.Database.EnsureDeleted();

                    // Create, migrate and seed the SQLite database
                    context.Database.Migrate();

                    var newCat = new Cat(){
                        MeowLoudness = 42
                    };
                    var newBreed = new CatBreed(){
                        BreedName = "SuperCat"
                    };

                    context.Cat.Add(newCat);
                    context.CatBreed.Add(newBreed);

                    // Sets the Ids on the above tracked entities.
                    await context.SaveChangesAsync();

                    // Save the line item creating the many-to-many relationship
                    context.CatBreedLine.Add(new CatBreedLine(){
                        CatId = newCat.Id,
                        CatBreedId = newBreed.Id
                    });
                    await context.SaveChangesAsync();

                    // The update! Bug: This bumps the createdon and updated dates even though this never changed!!!!

                    // Create a new VM of an existing cat to save
                    var catToSave = new CatUpdateViewModel()
                    {
                        Id = newCat.Id,
                        MeowLoudness = 100,
                        CatBreedIds = new List<int>{ newBreed.Id }
                    };

                    var existingCatEntity = await context.Cat
                        .SingleOrDefaultAsync(x => x.Id == newCat.Id);

                    // Mutate existingCatEntity
                    AutoMapper.Mapper.Map(catToSave, existingCatEntity);

                    await context.SaveChangesAsync();
               
                }

                Console.ReadKey();

                await host.StopAsync(TimeSpan.FromSeconds(5));
            }

        }


    }
}
