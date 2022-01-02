using System;
using System.IO;
using System.Text;

namespace FSGSave.CLI
{
    class Program
    {
        const string DefaultOutputFilename = "output.sav";

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                throw new ArgumentException(Resources.ErrorMessages.MustEnterInputFilename);
            }

            using (var inputStream = File.OpenRead(args[0]))
            {
                var magicBuffer = new byte[FSGSaveSection.MagicBytes.Length];
                inputStream.Read(magicBuffer);
                inputStream.Position = 0;

                ISerializer<FSGSaveSection> inputSerializer, outputSerializer;

                if (Encoding.ASCII.GetString(magicBuffer) == FSGSaveSection.Magic)
                {
                    inputSerializer = new FSGBinarySaveSerializer();
                    outputSerializer = new FSGXmlSaveSerializer();
                }
                else
                {
                    inputSerializer = new FSGXmlSaveSerializer();
                    outputSerializer = new FSGBinarySaveSerializer();
                }

                var save = inputSerializer.Deserialize(inputStream);
                var outputFilename = args.Length < 2 || string.IsNullOrWhiteSpace(args[1]) ? DefaultOutputFilename : args[1];

                using (var outputStream = File.OpenWrite(outputFilename))
                {
                    outputSerializer.Serialize(outputStream, save);
                }
            }
        }
    }
}
