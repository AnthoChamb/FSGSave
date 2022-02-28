using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSGSave
{
    public class FSGSession
    {
        public const string Magic = "SESS";

        public static byte[] MagicBytes { get => Encoding.ASCII.GetBytes(Magic); }

        public uint Id { get; }

        public string Name { get; set; }

        public uint InstanceId { get; }

        public string InstanceName { get; set; }

        public IEnumerable<FSGItemProperty> Items { get; set; }

        public int ItemCount { get => Items.Count(); }

        public IEnumerable<FSGArrayProperty> Arrays { get; set; }

        public int ArrayCount { get => Arrays.Count(); }

        public FSGSession(uint id, uint instanceId, IEnumerable<FSGItemProperty> items, IEnumerable<FSGArrayProperty> arrays)
        {
            Id = id;
            InstanceId = instanceId;
            Items = items;
            Arrays = arrays;
        }

        public override bool Equals(object obj)
        {
            return obj is FSGSession session &&
                Id.Equals(session.Id) &&
                InstanceId.Equals(session.InstanceId) &&
                Items.SequenceEqual(session.Items) &&
                Arrays.SequenceEqual(session.Arrays);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, InstanceId, Items, Arrays);
        }
    }
}
