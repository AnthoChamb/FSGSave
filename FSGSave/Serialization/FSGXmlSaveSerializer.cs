using System;
using System.IO;
using System.Linq;
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

            writer.WriteString(item.Type.ToString(item.Value));
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
                writer.WriteElementString(XmlElement.Value, array.ContainedType.ToString(value));
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
            var sessions = Enumerable.Empty<FSGSession>();

            if (!reader.IsEmptyElement)
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (reader.Name)
                            {
                                case XmlElement.Session:
                                    sessions = sessions.Append(DeserializeSession(reader));
                                    break;
                            }
                            break;
                        case XmlNodeType.EndElement:
                            switch (reader.Name)
                            {
                                case XmlElement.Section:
                                    return new FSGSaveSection(name, sessions, version);
                            }
                            break;
                    }
                }
            }

            return new FSGSaveSection(name, sessions, version);
        }

        private FSGSession DeserializeSession(XmlReader reader)
        {
            uint.TryParse(reader.GetAttribute(XmlAttribute.Id), out var id);
            uint.TryParse(reader.GetAttribute(XmlAttribute.InstanceId), out var instanceId);
            var name = reader.GetAttribute(XmlAttribute.Name);
            var instanceName = reader.GetAttribute(XmlAttribute.InstanceName);
            var items = Enumerable.Empty<FSGItemProperty>();
            var arrays = Enumerable.Empty<FSGArrayProperty>();

            if (!reader.IsEmptyElement)
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (reader.Name)
                            {
                                case XmlElement.Property:
                                    FSGPropertyType type = Enum.TryParse(reader.GetAttribute(XmlAttribute.Type), out type)
                                        ? type : throw new InvalidDataException(Resources.ErrorMessages.InvalidPropertyType);

                                    switch (type)
                                    {
                                        case FSGPropertyType.Collection:
                                            arrays = arrays.Append(DeserializeArray(reader));
                                            break;
                                        default:
                                            items = items.Append(DeserializeItem(reader, type));
                                            break;
                                    }
                                    break;
                            }
                            break;
                        case XmlNodeType.EndElement:
                            switch (reader.Name)
                            {
                                case XmlElement.Session:
                                    return new FSGSession(id, instanceId, items, arrays)
                                    {
                                        Name = name,
                                        InstanceName = instanceName
                                    };
                            }
                            break;
                    }
                }
            }

            return new FSGSession(id, instanceId, items, arrays)
            {
                Name = name,
                InstanceName = instanceName
            };
        }

        private FSGItemProperty DeserializeItem(XmlReader reader, FSGPropertyType type)
        {
            uint.TryParse(reader.GetAttribute(XmlAttribute.Id), out var id);
            var name = reader.GetAttribute(XmlAttribute.Name);
            var value = DeserializeValue(reader, type);

            return new FSGItemProperty(id, type, value)
            {
                Name = name
            };
        }

        private FSGArrayProperty DeserializeArray(XmlReader reader)
        {
            uint.TryParse(reader.GetAttribute(XmlAttribute.Id), out var id);
            var name = reader.GetAttribute(XmlAttribute.Name);
            FSGPropertyType containedType = Enum.TryParse(reader.GetAttribute(XmlAttribute.ContainedType), out containedType)
                ? containedType : throw new InvalidDataException(Resources.ErrorMessages.InvalidPropertyType);
            var values = Enumerable.Empty<object>();

            if (!reader.IsEmptyElement)
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (reader.Name)
                            {
                                case XmlElement.Value:
                                    values = values.Append(DeserializeValue(reader, containedType));
                                    break;
                            }
                            break;
                        case XmlNodeType.EndElement:
                            switch (reader.Name)
                            {
                                case XmlElement.Property:
                                    return new FSGArrayProperty(id, containedType, values)
                                    {
                                        Name = name
                                    };
                            }
                            break;
                    }
                }
            }

            return new FSGArrayProperty(id, containedType, values)
            {
                Name = name
            };
        }

        private object DeserializeValue(XmlReader reader, FSGPropertyType type)
        {
            if (!reader.IsEmptyElement)
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Text:
                            type.TryParse(reader.Value, out var value);
                            return value;
                        case XmlNodeType.EndElement:
                            switch (reader.Name)
                            {
                                case XmlElement.Property:
                                case XmlElement.Value:
                                    return type.GetDefaultValue();
                            }
                            break;
                    }
                }
            }

            return type.GetDefaultValue();
        }

        #endregion
    }
}
