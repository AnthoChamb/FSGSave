using System;
using System.IO;

namespace FSGSave.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                throw new ArgumentException(Resources.ErrorMessages.MustEnterInputFilename);
            }

            using (var inputStream = File.OpenRead(args[0]))
            {
                var binarySerializer = new FSGBinarySaveSerializer();
                var save = binarySerializer.Deserialize(inputStream);
                var outputFilename = string.Format("{0}.xml", args[0]);

                using (var outputStream = File.OpenWrite(outputFilename))
                {
                    var xmlSerializer = new FSGXmlSaveSerializer();
                    xmlSerializer.Serialize(outputStream, save);
                }
            }
        }
    }
}
