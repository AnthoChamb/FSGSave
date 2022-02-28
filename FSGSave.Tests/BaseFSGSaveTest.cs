using System.Collections.Generic;
using System.Linq;

namespace FSGSave.Tests
{
    public abstract class BaseFSGSaveTest
    {
        public const string SaveName = "Tests";
        public const int SaveVersion = 1;
        public const long SaveLength = 5 * 1024 * 1024;
        public const uint SessionId = 1U;
        public const uint InstanceId = 2U;
        public const uint ItemId = 3U;
        public const uint ArrayId = 4U;

        protected virtual FSGSaveSection CreateSaveSection()
        {
            var sessions = CreateSessions();

            var save = new FSGSaveSection(SaveName, sessions, SaveVersion)
            {
                Length = SaveLength
            };

            return save;
        }

        protected virtual IEnumerable<FSGSession> CreateSessions()
        {
            var sessions = Enumerable.Empty<FSGSession>();
            var session = CreateSession();
            sessions = sessions.Append(session);
            return sessions;
        }

        protected virtual FSGSession CreateSession()
        {
            var items = CreateItems();
            var arrays = CreateArrays();
            return new FSGSession(SessionId, InstanceId, items, arrays);
        }

        protected virtual IEnumerable<FSGItemProperty> CreateItems()
        {
            var items = Enumerable.Empty<FSGItemProperty>();

            foreach (var type in FSGProperty.ValuePropertyTypes)
            {
                var item = CreateItem(type);
                items = items.Append(item);
            }

            return items;
        }

        protected virtual FSGItemProperty CreateItem(FSGPropertyType type)
        {
            var value = type.GetDefaultValue();
            return new FSGItemProperty(ItemId, type, value);
        }

        protected virtual IEnumerable<FSGArrayProperty> CreateArrays()
        {
            var arrays = Enumerable.Empty<FSGArrayProperty>();

            foreach (var type in FSGProperty.ValuePropertyTypes)
            {
                var array = CreateArray(type);
                arrays = arrays.Append(array);
            }

            return arrays;
        }

        protected virtual FSGArrayProperty CreateArray(FSGPropertyType containedType)
        {
            var values = Enumerable.Empty<object>();
            var value = containedType.GetDefaultValue();
            values = values.Append(value);
            return new FSGArrayProperty(ArrayId, containedType, values);
        }
    }
}
