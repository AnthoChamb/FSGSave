using System;
using System.Collections.Generic;

namespace FSGSave
{
    public abstract class FSGProperty
    {
        private static IEnumerable<FSGPropertyType> valuePropertyTypes;

        public static IEnumerable<FSGPropertyType> ValuePropertyTypes
        {
            get
            {
                if (valuePropertyTypes == null)
                {
                    valuePropertyTypes = new FSGPropertyType[]
                    {
                        FSGPropertyType.Bool,
                        FSGPropertyType.Int,
                        FSGPropertyType.Uint,
                        FSGPropertyType.Uint64,
                        FSGPropertyType.Float
                    };
                }

                return valuePropertyTypes;
            }
        }

        public uint Id { get; }

        public string Name { get; set; }

        public abstract FSGPropertyType Type { get; }

        public FSGProperty(uint id)
        {
            Id = id;
        }

        public override abstract bool Equals(object obj);

        public override abstract int GetHashCode();
    }
}
