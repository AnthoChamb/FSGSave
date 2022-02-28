using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSGSave.Tests
{
    [TestClass]
    public class FSGBinarySaveSerializerTest : BaseFSGSaveSerializerTest
    {
        protected override ISerializer<FSGSaveSection> CreateSerializer()
        {
            return new FSGBinarySaveSerializer();
        }
    }
}
