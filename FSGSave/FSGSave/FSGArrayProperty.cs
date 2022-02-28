using System;
using System.Collections.Generic;
using System.Linq;

namespace FSGSave
{
    public class FSGArrayProperty : FSGProperty
    {
        public override FSGPropertyType Type { get => FSGPropertyType.Collection; }

        public FSGPropertyType ContainedType { get; }

        public object[] Values { get; }

        public int Count { get => Values.Length; }

        public FSGArrayProperty(uint id, FSGPropertyType containedType, object[] values)
            : base(id)
        {
            ContainedType = containedType;
            Values = values;
        }

        public FSGArrayProperty(uint id, FSGPropertyType containedType, int count)
            : this(id, containedType, new object[count]) { }
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
