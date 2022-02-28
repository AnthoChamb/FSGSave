using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSGSave.Tests
{
    [TestClass]
    public class FSGXmlSaveSerializerTest : BaseFSGSaveSerializerTest
    {
        protected override ISerializer<FSGSaveSection> CreateSerializer()
        {
            return new FSGXmlSaveSerializer();
        }
    }
}
