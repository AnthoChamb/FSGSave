using System;

namespace FSGSave
{
    public class FSGItemProperty : FSGProperty
    {
        public override FSGPropertyType Type { get; }

        public object Value { get; set; }

        public FSGItemProperty(uint id, FSGPropertyType type, object value)
            : base(id)
        {
            if (type == FSGPropertyType.Collection)
            {
                throw new ArgumentException(Resources.ErrorMessages.InvalidPropertyType);
            }

            Type = type;
            Value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is FSGItemProperty item &&
                Id.Equals(item.Id) &&
                Type.Equals(item.Type) &&
                Value.Equals(item.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Type, Value);
        }
    }
}
