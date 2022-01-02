﻿namespace FSGSave
{
    public abstract class FSGProperty
    {
        public uint Id { get; }

        public string Name { get; set; }

        public abstract FSGPropertyType Type { get; }

        public FSGProperty(uint id)
        {
            Id = id;
        }
    }
}