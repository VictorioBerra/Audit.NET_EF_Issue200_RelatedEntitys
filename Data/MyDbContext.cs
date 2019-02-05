using Microsoft.EntityFrameworkCore;
using EFCore.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Design;
using EFCore;
using Audit.EntityFramework;
using Audit.Core;
using System;
using System.Linq;
using System.Collections.Generic;

namespace EfCore.Data
{
    public class MyAppContextFactory : IDesignTimeDbContextFactory<MyAppContext>
    {
        public MyAppContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MyAppContext>();
            optionsBuilder.UseSqlite(Constants.ConnectionString);

            return new MyAppContext(optionsBuilder.Options);
        }
    }

    public class MyAppContext : AuditDbContext
    {
        public DbSet<GenericAudit> GenericAudit { get; set; }

        public DbSet<Cat> Cat { get; set; }

        public DbSet<CatBreed> CatBreed { get; set; }


        public MyAppContext(DbContextOptions<MyAppContext> options)
            : base(options)
        { }

        public override void OnScopeCreated(AuditScope auditScope)
        {

            // It is possible things like the integration tests will not happen in the context of a user.
            var currentWUPeopleIdString = "Anonymous";
            var currentUsernameString = "Anonymous";

            var entities = auditScope.GetEntityFrameworkEvent().Entries.Where(x => x.Action == "Insert" || x.Action == "Update");
            foreach (var entity in entities)
            {
                IAuditable auditableEntry = entity.Entity as IAuditable;
                if (auditableEntry != null)
                {
                    // entity.GetEntry().CurrentValues, etc...
                    if (entity.Action == "Insert")
                    {
                        auditableEntry.CreatedOnUtc = DateTime.UtcNow;
                        auditableEntry.CreatedByWUPeopleId = currentWUPeopleIdString;
                        auditableEntry.CreatedByDisplayName = currentUsernameString;
                    }

                    auditableEntry.UpdatedOnUtc = DateTime.UtcNow;
                    auditableEntry.UpdatedByWUPeopleId = currentWUPeopleIdString;
                    auditableEntry.UpdatedByDisplayName = currentUsernameString;
                }

            }
        }
    }
}