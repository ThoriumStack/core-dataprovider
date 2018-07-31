using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyBucks.Core.Model.DataModel;

namespace MyBucks.Core.DataProvider
{
    public class ContextBase : DbContext
    {
        private EfStateTracker _stateTracker;

        public ContextBase() : base()
        {
            _stateTracker = new EfStateTracker();
        }

        public ContextBase(string connectionString) : base()
        {
            ConnectionString = connectionString;
            _stateTracker = new EfStateTracker();
        }
        
        public string ConnectionString { get; set; }
        public string CurrentUserId { get; set; }
        public string CurrentContext { get; set; }

        public override int SaveChanges()
        {
            var changes = this.ChangeTracker.Entries()
                .Where(c => c.State == EntityState.Modified || c.State == EntityState.Added || c.State == EntityState.Deleted)
                .ToList();
           
            _stateTracker.TriggerStateTrackers(changes);
            
            PreSaveLogic();
            
            var result = base.SaveChanges();

            return result;
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            PreSaveLogic();
                
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            PreSaveLogic();
                
            return base.SaveChangesAsync(cancellationToken);
        }

        private void PreSaveLogic()
        {
            var baseContextEntries = this.ChangeTracker.Entries<BaseContextModel>().Where(x => x.State == EntityState.Added).ToArray();
            foreach (var entry in baseContextEntries)
            {
                entry.Entity.Context = CurrentContext;
            }
            
            var baseEntries = this.ChangeTracker.Entries<BaseModel>()
                .Where(x => x.State != EntityState.Unchanged)
                .ToArray();
            
            foreach (var entry in baseEntries.Where(x => x.State == EntityState.Modified))
            {
                entry.Entity.ModifiedById = CurrentUserId;
                entry.Entity.ModifiedDate = DateTime.Now;
            }
            
            foreach (var entry in baseEntries.Where(x => x.State == EntityState.Added))
            {
                entry.Entity.CreatedById = CurrentUserId;
                entry.Entity.CreatedDate = DateTime.Now;
            }
        }
    }
}
