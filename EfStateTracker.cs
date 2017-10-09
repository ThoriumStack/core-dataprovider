using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MyBucks.Core.DataProvider.Model;

namespace MyBucks.Core.DataProvider
{
    public class EfStateTracker
    {
        private List<IEntityStateTracker> EntityStateTrackers { get; set; } = new List<IEntityStateTracker>();

        internal void TriggerStateTrackers(List<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry> changes)
        {
            if (changes?.Any() ?? false)
            {
                var entityChanges = changes.Select(c => new EntityChange { Entity = c.Entity, State = (EfEntityState)c.State, Fields = GetChangedFields(c) }).Distinct().ToList();
                HandleChange(entityChanges);
            }
        }

        // todo: remove, no longer supported
        //internal void TriggerStateTrackers(List<System.Data.Entity.Infrastructure.DbEntityEntry> changes)
        //{
        //    if (changes.Any())
        //    {
        //        var entityChanges = changes.Select(c => new EntityChange { Entity = c.Entity, State = MapEfStates(c.State), Fields = GetChangedFields(c) }).Distinct().ToList();
        //        HandleChange(entityChanges);
        //    }
        //}


        private List<string> GetChangedFields(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry change)
        {

            
            var result = new List<string>();

            if (change.State != Microsoft.EntityFrameworkCore.EntityState.Modified)
            {
                return result;
            }

            foreach (var prop in change.OriginalValues.Properties)
            {
                var originalValue = change.OriginalValues[prop]?.ToString();
                var currentValue = change.CurrentValues[prop]?.ToString();
                if (originalValue != currentValue) //Only create a log if the value changes
                {
                    result.Add(prop.Name);
                }
            }
            return result;
        }

        // todo: remove, no longer supported

        //private List<string> GetChangedFields(System.Data.Entity.Infrastructure.DbEntityEntry change)
        //{


        //    var result = new List<string>();

        //    if (change.State != EntityState.Modified)
        //    {
        //        return result;
        //    }


        //    foreach (var prop in change.OriginalValues.PropertyNames)
        //    {
        //        var originalValue = change.OriginalValues[prop]?.ToString();
        //        var currentValue = change.CurrentValues[prop]?.ToString();
        //        if (originalValue != currentValue) //Only create a log if the value changes
        //        {
        //            result.Add(prop);
        //        }
        //    }
        //    return result;
        //}

        private EfEntityState MapEfStates(EntityState state)
        {
            //(Modules.DataProvider.EfEntityState)c.State

            var stateMap = new Dictionary<EntityState, EfEntityState> {
                [EntityState.Added] = EfEntityState.Added,
                [EntityState.Deleted] = EfEntityState.Deleted,
                [EntityState.Detached] = EfEntityState.Detached,
                [EntityState.Modified] = EfEntityState.Modified,
                [EntityState.Unchanged] = EfEntityState.Unchanged,
            };

            return stateMap[state];
        }

        private void HandleChange(List<EntityChange> entityChanges)
        {
            if (!entityChanges?.Any() ?? false)
            {
                // no changes
                return;
            }
            foreach (var item in entityChanges)
            {
                var entityType = item.Entity.GetType();
                
                EntityStateTrackers
                    .Where(c =>
                        c.EntityTypes != null &&
                        c.EntityTypes.Select(d => d.Name).Contains(entityType.Name)
                    ).ToList()
                    .ForEach(f => f.HandleEntityChange(entityType, item.Entity, item.State, item.Fields));
            }
        }

        public void SubscribeToEntityChanges(IEntityStateTracker tracker)
        {
            EntityStateTrackers.Add(tracker);
        }
    }
}
