using System.Collections.Generic;

namespace MyBucks.Core.DataProvider.Model
{
    public class EntityChange
    {
        public object Entity { get; internal set; }
        public EfEntityState State { get; internal set; }
        public List<string> Fields { get; set; }
    }
}
