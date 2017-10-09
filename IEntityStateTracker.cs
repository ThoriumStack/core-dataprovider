using MyBucks.Core.DataProvider.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyBucks.Core.DataProvider
{
    public interface IEntityStateTracker
    {
        List<Type> EntityTypes { get; set; }


        void HandleEntityChange<T>(Type entityType, T entity, EfEntityState state, List<string> fields);
    }
}
