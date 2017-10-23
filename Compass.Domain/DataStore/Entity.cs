using System;

namespace Compass.Domain.DataStore
{
    public abstract class Entity
    {
        public virtual Guid Identifier { get; set; }
        public abstract string DocType { get; }
    }
}
