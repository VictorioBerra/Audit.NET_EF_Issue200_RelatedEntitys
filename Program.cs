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

                    context.Cat.Add(new Cat(){
                        MeowLoudness = 42
                    });

                    context.SaveChanges();
               
                }

                Console.ReadKey();

                await host.StopAsync(TimeSpan.FromSeconds(5));
            }

        }


    }
}
