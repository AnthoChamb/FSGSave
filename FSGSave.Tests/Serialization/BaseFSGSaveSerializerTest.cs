using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace FSGSave.Tests
{
    [TestClass]
    public abstract class BaseFSGSaveSerializerTest : BaseFSGSaveTest
    {
        protected abstract ISerializer<FSGSaveSection> CreateSerializer();

        [TestMethod]
        public void Serialize_StreamLengthEqualsLength()
        {
            var save = CreateSaveSection();
            var serializer = CreateSerializer();
            using var stream = new MemoryStream();

            serializer.Serialize(stream, save);

            Assert.AreEqual(save.Length, stream.Length);
        }

        [TestMethod]
        public void Deserialize_LengthEqualsStreamLength()
        {
            var value = CreateSaveSection();
            var serializer = CreateSerializer();
            using var stream = new MemoryStream();

            serializer.Serialize(stream, value);
            stream.Position = 0;
            var result = serializer.Deserialize(stream);

            Assert.AreEqual(stream.Length, result.Length);
        }

        [TestMethod]
        public void SerializeAndDeserialize_ResultEqualsValue()
        {
            var value = CreateSaveSection();
            var serializer = CreateSerializer();
            using var stream = new MemoryStream();

            serializer.Serialize(stream, value);
            stream.Position = 0;
            var result = serializer.Deserialize(stream);

            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void SerializeAndDeserialize_WithEmptySessions_ResultEqualsValue()
        {
            var value = CreateSaveSection();
            value.Sessions = Enumerable.Empty<FSGSession>();

            var serializer = CreateSerializer();
            using var stream = new MemoryStream();

            serializer.Serialize(stream, value);
            stream.Position = 0;
            var result = serializer.Deserialize(stream);

            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void SerializeAndDeserialize_WithEmptyProperties_ResultEqualsValue()
        {
            var value = CreateSaveSection();

            foreach (var session in value.Sessions)
            {
                session.Items = Enumerable.Empty<FSGItemProperty>();
                session.Arrays = Enumerable.Empty<FSGArrayProperty>();
            }

            var serializer = CreateSerializer();
            using var stream = new MemoryStream();

            serializer.Serialize(stream, value);
            stream.Position = 0;
            var result = serializer.Deserialize(stream);

            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void SerializeAndDeserialize_WithEmptyValues_ResultEqualsValue()
        {
            var value = CreateSaveSection();

            foreach (var session in value.Sessions)
            {
                foreach (var array in session.Arrays)
                {
                    array.Values = Enumerable.Empty<object>();
                }
            }

            var serializer = CreateSerializer();
            using var stream = new MemoryStream();

            serializer.Serialize(stream, value);
            stream.Position = 0;
            var result = serializer.Deserialize(stream);

            Assert.AreEqual(value, result);
        }
    }
}
