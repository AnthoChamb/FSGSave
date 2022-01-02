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

        #region Serialization

        public void Serialize(Stream stream, FSGSaveSection value)
        {
            using (var writer = XmlWriter.Create(stream))
            {
                SerializeSection(writer, value);
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

        public FSGSaveSection Deserialize(Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}
