using System;
using System.IO;
using System.Linq;
using System.Text;

namespace FSGSave
{
    public class FSGBinarySaveSerializer : ISerializer<FSGSaveSection>
    {
        #region Serialize

        public void Serialize(Stream stream, FSGSaveSection value)
        {
            using (var writer = new EndianBinaryWriter(stream, Encoding.ASCII, true, Endianness.Big))
            {
                SerializeSection(writer, value);

                if (value.Length.HasValue && stream.Length < value.Length)
                {
                    stream.SetLength(value.Length.Value);
                }
            }
        }

        private void SerializeSection(BinaryWriter writer, FSGSaveSection section)
        {
            writer.Write(FSGSaveSection.MagicBytes);
            writer.Write(0x0100);
            writer.Write(section.Version);

            var nameBuffer = new byte[FSGSaveSection.NameBufferLength];
            Encoding.ASCII.GetBytes(section.Name, nameBuffer);
            writer.Write(nameBuffer);
            writer.Write(section.SessionCount);

            foreach (var session in section.Sessions)
            {
                SerializeSession(writer, session);
            }
        }

        private void SerializeSession(BinaryWriter writer, FSGSession session)
        {
            writer.Write(FSGSession.MagicBytes);
            writer.Write(session.Id);
            writer.Write(session.InstanceId);
            writer.Write(session.ItemCount);
            writer.Write(session.ArrayCount);

            foreach (var item in session.Items)
            {
                SerializeItem(writer, item);
            }

            foreach (var array in session.Arrays)
            {
                SerializeArray(writer, array);
            }
        }

        private void SerializeItem(BinaryWriter writer, FSGItemProperty item)
        {
            writer.Write(item.Id);
            writer.Write((int)item.Type);
            SerializeValue(writer, item.Type, item.Value);
        }

        private void SerializeArray(BinaryWriter writer, FSGArrayProperty array)
        {
            writer.Write(array.Id);
            writer.Write((int)array.Type);
            writer.Write((int)array.ContainedType);
            writer.Write(array.Count);

            foreach (var value in array.Values)
            {
                SerializeValue(writer, array.ContainedType, value);
            }
        }

        private void SerializeValue(BinaryWriter writer, FSGPropertyType type, object value)
        {
            switch (type)
            {
                case FSGPropertyType.Bool:
                    writer.Write((bool)value);
                    break;
                case FSGPropertyType.Int:
                    writer.Write((int)value);
                    break;
                case FSGPropertyType.Uint:
                    writer.Write((uint)value);
                    break;
                case FSGPropertyType.Uint64:
                    writer.Write((ulong)value);
                    break;
                case FSGPropertyType.Float:
                    writer.Write((float)value);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region Deserialize

        public FSGSaveSection Deserialize(Stream stream)
        {
            using (var reader = new EndianBinaryReader(stream, Encoding.ASCII, true, Endianness.Big))
            {
                var section = DeserializeSection(reader);
                section.Length = stream.Length;
                return section;
            }
        }

        private FSGSaveSection DeserializeSection(BinaryReader reader)
        {
            var magicBuffer = reader.ReadBytes(FSGSaveSection.MagicBytes.Length);
            if (!magicBuffer.SequenceEqual(FSGSaveSection.MagicBytes))
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
            var magicBuffer = reader.ReadBytes(FSGSession.MagicBytes.Length);
            if (!magicBuffer.SequenceEqual(FSGSession.MagicBytes))
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
