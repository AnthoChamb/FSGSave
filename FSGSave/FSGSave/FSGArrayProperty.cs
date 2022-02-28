using System;
using System.Collections.Generic;
using System.Linq;

namespace FSGSave
{
    public class FSGArrayProperty : FSGProperty
    {
        public override FSGPropertyType Type { get => FSGPropertyType.Collection; }

        public FSGPropertyType ContainedType { get; }

        public IEnumerable<object> Values { get; set; }

        public int Count { get => Values.Count(); }

        public FSGArrayProperty(uint id, FSGPropertyType containedType, IEnumerable<object> values)
            : base(id)
        {
            ContainedType = containedType;
            Values = values;
        }

        public override bool Equals(object obj)
        {
            return obj is FSGArrayProperty array &&
                Id.Equals(array.Id) &&
                ContainedType.Equals(array.ContainedType) &&
                Values.SequenceEqual(array.Values);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, ContainedType, Values);
        }
    }
}
