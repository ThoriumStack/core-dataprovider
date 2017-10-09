using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyBucks.Core.DataProvider
{
    public class ContextBase : DbContext
    {
        private EfStateTracker _stateTracker;

        public ContextBase() : base()
        {

        }

        public ContextBase(string connectionString) : base()
        {
            ConnectionString = connectionString;
        }
        
        public string ConnectionString { get; set; }

        public override int SaveChanges()
        {
            var changes = this.ChangeTracker.Entries()
                .Where(c => c.State == EntityState.Modified || c.State == EntityState.Added || c.State == EntityState.Deleted)
                .ToList();
           
            _stateTracker.TriggerStateTrackers(changes);
            var result = base.SaveChanges();

            return result;
        }
    }
}
