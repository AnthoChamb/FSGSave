using System;
using System.IO;
using System.Text;

namespace FSGSave
{
    public class FSGBinarySaveSerializer : ISerializer<FSGSaveSection>
    {
        public void Serialize(Stream stream, FSGSaveSection value)
        {
            throw new NotImplementedException();
        }

        #region Deserialize

        public FSGSaveSection Deserialize(Stream stream)
        {
            using (var reader = new EndianBinaryReader(stream, Encoding.ASCII, Endianness.Big))
            {
                return DeserializeSection(reader);
            }
        }

        private FSGSaveSection DeserializeSection(BinaryReader reader)
        {
            var magic = Encoding.ASCII.GetString(reader.ReadBytes(FSGSaveSection.MagicBytes.Length));
            if (magic != FSGSaveSection.Magic)
            {
                throw new InvalidDataException(Resources.ErrorMessages.InvalidMagic);
            }
            reader.ReadInt32(); // Always 0x00 0x00 0x01 0x00

            var version = reader.ReadInt32();
            var name = Encoding.ASCII.GetString(reader.ReadBytes(FSGSaveSection.NameBufferLength)).Trim('\0');
            var sessionCount = reader.ReadInt32();

            var sessions = new FSGSession[sessionCount];
            for (var i = 0; i < sessionCount; i++)
            {
                sessions[i] = DeserializeSession(reader);
            }

            return new FSGSaveSection(name, sessions, version);
        }

        private FSGSession DeserializeSession(BinaryReader reader)
        {
            var magic = Encoding.ASCII.GetString(reader.ReadBytes(FSGSession.MagicBytes.Length));
            if (magic != FSGSession.Magic)
            {
                throw new InvalidDataException(Resources.ErrorMessages.InvalidMagic);
            }

            var id = reader.ReadUInt32();
            var instanceId = reader.ReadUInt32();
            var itemCount = reader.ReadInt32();
            var arrayCount = reader.ReadInt32();

            var items = new FSGItemProperty[itemCount];
            for (var i = 0; i < itemCount; i++)
            {
                items[i] = DeserializeItem(reader);
            }

            var arrays = new FSGArrayProperty[arrayCount];
            for (var i = 0; i < arrayCount; i++)
            {
                arrays[i] = DeserializeArray(reader);
            }

            return new FSGSession(id, instanceId, items, arrays);
        }

        private FSGItemProperty DeserializeItem(BinaryReader reader)
        {
            var id = reader.ReadUInt32();
            var type = (FSGPropertyType)reader.ReadInt32();
            var value = DeserializeValue(reader, type);

            return new FSGItemProperty(id, type, value);
        }

        private FSGArrayProperty DeserializeArray(BinaryReader reader)
        {
            var id = reader.ReadUInt32();
            var type = (FSGPropertyType)reader.ReadInt32();
            if (type != FSGPropertyType.Collection)
            {
                throw new InvalidDataException(Resources.ErrorMessages.InvalidPropertyType);
            }

            var containedType = (FSGPropertyType)reader.ReadInt32();
            var count = reader.ReadInt32();

            var values = new object[count];
            for (var i = 0; i < count; i++)
            {
                values[i] = DeserializeValue(reader, containedType);
            }

            return new FSGArrayProperty(id, containedType, values);
        }

        private object DeserializeValue(BinaryReader reader, FSGPropertyType type)
        {
            switch (type)
            {
                case FSGPropertyType.Bool:
                    return reader.ReadBoolean();
                case FSGPropertyType.Int:
                    return reader.ReadInt32();
                case FSGPropertyType.Uint:
                    return reader.ReadUInt32();
                case FSGPropertyType.Uint64:
                    return reader.ReadUInt64();
                case FSGPropertyType.Float:
                    return reader.ReadSingle();
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion
    }
}
