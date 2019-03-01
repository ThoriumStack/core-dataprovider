using System;
using System.Collections.Generic;
using Thorium.Core.DataProvider.Model;

namespace Thorium.Core.DataProvider
{
    public interface IEntityStateTracker
    {
        List<Type> EntityTypes { get; set; }


        void HandleEntityChange<T>(Type entityType, T entity, EfEntityState state, List<string> fields);
    }
}
