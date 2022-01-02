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
    }
}
