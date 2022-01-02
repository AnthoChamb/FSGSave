using System;

namespace FSGSave
{
    public class FSGItemProperty : FSGProperty
    {
        public override FSGPropertyType Type { get; }

        public object Value { get; }

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
    }
}
