using System;
using System.IO;
using System.Xml;

namespace FSGSave
{
    public class FSGXmlSaveSerializer : ISerializer<FSGSaveSection>
    {
        private class XmlElement
        {
            public const string Section = "SaveSection";
            public const string Session = "Session";
            public const string Property = "Property";
            public const string Value = "Value";
        }

        private class XmlAttribute
        {
            public const string Id = "ID";
            public const string Name = "Name";
            public const string InstanceId = "InstanceID";
            public const string InstanceName = "InstanceName";
            public const string Version = "Version";
            public const string Count = "Count";
            public const string SessionCount = "SessionCount";
            public const string ItemCount = "ItemCount";
            public const string ArrayCount = "ArrayCount";
            public const string Type = "Type";
            public const string ContainedType = "ContainedType";
        }

        #region Serialize

        public void Serialize(Stream stream, FSGSaveSection value)
        {
            using (var writer = XmlWriter.Create(stream))
            {
                SerializeSection(writer, value);

                if (value.Length.HasValue && stream.Length < value.Length)
                {
                    stream.SetLength(value.Length.Value);
                }
            }
        }

        private void SerializeSection(XmlWriter writer, FSGSaveSection section)
        {
            writer.WriteStartElement(XmlElement.Section);
            writer.WriteAttributeString(XmlAttribute.Name, section.Name);
            writer.WriteAttributeString(XmlAttribute.Version, section.Version.ToString());
            writer.WriteAttributeString(XmlAttribute.SessionCount, section.SessionCount.ToString());

            foreach (var session in section.Sessions)
            {
                SerializeSession(writer, session);
            }

            writer.WriteEndElement();
        }

        private void SerializeSession(XmlWriter writer, FSGSession session)
        {
            writer.WriteStartElement(XmlElement.Session);
            writer.WriteAttributeString(XmlAttribute.Id, session.Id.ToString());
            writer.WriteAttributeString(XmlAttribute.InstanceId, session.InstanceId.ToString());
            writer.WriteAttributeString(XmlAttribute.InstanceName, session.InstanceName);
            writer.WriteAttributeString(XmlAttribute.Name, session.Name);
            writer.WriteAttributeString(XmlAttribute.ItemCount, session.ItemCount.ToString());
            writer.WriteAttributeString(XmlAttribute.ArrayCount, session.ArrayCount.ToString());

            foreach (var item in session.Items)
            {
                SerializeItem(writer, item);
            }

            foreach (var array in session.Arrays)
            {
                SerializeArray(writer, array);
            }

            writer.WriteEndElement();
        }

        private void SerializeItem(XmlWriter writer, FSGItemProperty item)
        {
            writer.WriteStartElement(XmlElement.Property);
            writer.WriteAttributeString(XmlAttribute.Id, item.Id.ToString());
            writer.WriteAttributeString(XmlAttribute.Type, item.Type.ToString());
            writer.WriteAttributeString(XmlAttribute.Name, item.Name);

            writer.WriteString(item.Value.ToString());
            writer.WriteEndElement();
        }

        private void SerializeArray(XmlWriter writer, FSGArrayProperty array)
        {
            writer.WriteStartElement(XmlElement.Property);
            writer.WriteAttributeString(XmlAttribute.Id, array.Id.ToString());
            writer.WriteAttributeString(XmlAttribute.Type, array.Type.ToString());
            writer.WriteAttributeString(XmlAttribute.Name, array.Name);
            writer.WriteAttributeString(XmlAttribute.ContainedType, array.ContainedType.ToString());
            writer.WriteAttributeString(XmlAttribute.Count, array.Count.ToString());

            foreach (var value in array.Values)
            {
                writer.WriteElementString(XmlElement.Value, value.ToString());
            }

            writer.WriteEndElement();
        }

        #endregion

        #region Deserialize

        public FSGSaveSection Deserialize(Stream stream)
        {
            using (var reader = XmlReader.Create(stream))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (reader.Name)
                            {
                                case XmlElement.Section:
                                    var section = DeserializeSection(reader);
                                    section.Length = stream.Length;
                                    return section;
                            }
                            break;
                    }
                }
            }

            return null;
        }

        private FSGSaveSection DeserializeSection(XmlReader reader)
        {
            var name = reader.GetAttribute(XmlAttribute.Name);
            int version = int.TryParse(reader.GetAttribute(XmlAttribute.Version), out version) ? version : 1;
            int sessionCount = int.TryParse(reader.GetAttribute(XmlAttribute.SessionCount), out sessionCount) ? sessionCount : 0;
            var sessions = new FSGSession[sessionCount];
            var i = 0;

            while (i < sessionCount && reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (reader.Name)
                        {
                            case XmlElement.Session:
                                sessions[i++] = DeserializeSession(reader);
                                break;
                        }
                        break;
                }
            }

            return new FSGSaveSection(name, sessions, version);
        }

        private FSGSession DeserializeSession(XmlReader reader)
        {
            uint id = uint.TryParse(reader.GetAttribute(XmlAttribute.Id), out id) ? id : 0;
            uint instanceId = uint.TryParse(reader.GetAttribute(XmlAttribute.InstanceId), out instanceId) ? instanceId : 0;
            var name = reader.GetAttribute(XmlAttribute.Name);
            var instanceName = reader.GetAttribute(XmlAttribute.InstanceName);
            int itemCount = int.TryParse(reader.GetAttribute(XmlAttribute.ItemCount), out itemCount) ? itemCount : 0;
            int arrayCount = int.TryParse(reader.GetAttribute(XmlAttribute.ArrayCount), out arrayCount) ? arrayCount : 0;
            var items = new FSGItemProperty[itemCount];
            var arrays = new FSGArrayProperty[arrayCount];
            var itemIndex = 0;
            var arrayIndex = 0;

            while ((itemIndex < itemCount || arrayIndex < arrayCount) && reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (reader.Name)
                        {
                            case XmlElement.Property:
                                if (itemIndex < itemCount)
                                {
                                    items[itemIndex++] = DeserializeItem(reader);
                                }
                                else
                                {
                                    arrays[arrayIndex++] = DeserializeArray(reader);
                                }
                                break;
                        }
                        break;
                }
            }

            return new FSGSession(id, instanceId, items, arrays)
            {
                Name = name,
                InstanceName = instanceName
            };
        }

        private FSGItemProperty DeserializeItem(XmlReader reader)
        {
            uint id = uint.TryParse(reader.GetAttribute(XmlAttribute.Id), out id) ? id : 0;
            var name = reader.GetAttribute(XmlAttribute.Name);
            FSGPropertyType type = Enum.TryParse(reader.GetAttribute(XmlAttribute.Type), out type)
                ? type : throw new InvalidDataException(Resources.ErrorMessages.InvalidPropertyType);
            var value = DeserializeValue(reader, type);

            return new FSGItemProperty(id, type, value)
            {
                Name = name
            };
        }

        private FSGArrayProperty DeserializeArray(XmlReader reader)
        {
            if (!Enum.TryParse(reader.GetAttribute(XmlAttribute.Type), out FSGPropertyType type) || type != FSGPropertyType.Collection)
            {
                throw new InvalidDataException(Resources.ErrorMessages.InvalidPropertyType);
            }

            uint id = uint.TryParse(reader.GetAttribute(XmlAttribute.Id), out id) ? id : 0;
            var name = reader.GetAttribute(XmlAttribute.Name);
            FSGPropertyType containedType = Enum.TryParse(reader.GetAttribute(XmlAttribute.ContainedType), out containedType) 
                ? containedType : throw new InvalidDataException(Resources.ErrorMessages.InvalidPropertyType);
            int count = int.TryParse(reader.GetAttribute(XmlAttribute.Count), out count) ? count : 0;
            var values = new object[count];
            var i = 0;

            while (i < count && reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (reader.Name)
                        {
                            case XmlElement.Value:
                                values[i++] = DeserializeValue(reader, containedType);
                                break;
                        }
                        break;
                }
            }

            return new FSGArrayProperty(id, containedType, values)
            {
                Name = name
            };
        }

        private object DeserializeValue(XmlReader reader, FSGPropertyType type)
        {
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Text:
                        switch (type)
                        {
                            case FSGPropertyType.Bool:
                                return bool.TryParse(reader.Value, out var boolValue) ? boolValue : false;
                            case FSGPropertyType.Int:
                                return int.TryParse(reader.Value, out var intValue) ? intValue : 0;
                            case FSGPropertyType.Uint:
                                return uint.TryParse(reader.Value, out var uintValue) ? uintValue : 0;
                            case FSGPropertyType.Uint64:
                                return ulong.TryParse(reader.Value, out var uint64Value) ? uint64Value : 0;
                            case FSGPropertyType.Float:
                                return float.TryParse(reader.Value, out var floatValue) ? floatValue : 0.0;
                            default:
                                throw new NotImplementedException();
                        }
                    case XmlNodeType.EndElement:
                        throw new InvalidDataException(Resources.ErrorMessages.ValueNotFound);
                }
            }

            throw new InvalidDataException(Resources.ErrorMessages.ValueNotFound);
        }

        #endregion
    }
}
