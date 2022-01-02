namespace FSGSave
{
    public class FSGItemProperty : FSGProperty
    {
        public override FSGPropertyType Type { get; }

        public object Value { get; }

        public FSGItemProperty(uint id, FSGPropertyType type, object value)
            : base(id)
        {
            Type = type;
            Value = value;
        }
    }
}
