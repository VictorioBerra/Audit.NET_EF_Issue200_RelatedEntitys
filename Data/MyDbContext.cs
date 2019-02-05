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
        public MyAppContext(DbContextOptions<MyAppContext> options)
            : base(options)
        { }

        public DbSet<GenericAudit> GenericAudit { get; set; }
        public DbSet<Cat> Cat { get; set; }
        public DbSet<CatBreed> CatBreed { get; set; }

        public override void OnScopeCreated(AuditScope auditScope)
        {

            // It is possible things like the integration tests will not happen in the context of a user.
            var currentWUPeopleIdString = "Anonymous";
            var currentUsernameString = "Anonymous";

            var efEvent = auditScope.GetEntityFrameworkEvent();
            var entries = efEvent.Entries.Where(x => x.Action == "Insert" || x.Action == "Update");
            foreach (var entry in entries)
            {
                IAuditable auditableEntry = entry.Entity as IAuditable;
                if (auditableEntry != null)
                {
                    // entity.GetEntry().CurrentValues, etc...
                    if (entry.Action == "Insert")
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

            // Re-validate  (we have to completely overwrite the Entries because we have no way to update the existing entry and fix the ColumnValues)
            // If `GetColumnValues()` was public, that and `DbContextHelper.GetValidationResults(auditableEntry)` would be enough.
            var _helper = new DbContextHelper();
            var eventAsEFEvent = _helper.CreateAuditEvent(this);
            efEvent.Entries = eventAsEFEvent.Entries;
        }
    }
}