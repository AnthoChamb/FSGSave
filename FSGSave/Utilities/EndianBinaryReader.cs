using System;
using System.IO;
using System.Text;

namespace FSGSave
{
    public class EndianBinaryReader : BinaryReader
    {
        public Endianness Endianness { get; }

        public EndianBinaryReader(Stream input) : this(input, Endianness.Little) { }

        public EndianBinaryReader(Stream input, Encoding encoding) : this(input, encoding, Endianness.Little) { }

        public EndianBinaryReader(Stream input, Encoding encoding, bool leaveOpen)
            : this(input, encoding, leaveOpen, Endianness.Little) { }

        public EndianBinaryReader(Stream input, Endianness endianness) : base(input)
        {
            Endianness = endianness;
        }

        public EndianBinaryReader(Stream input, Encoding encoding, Endianness endianness) : base(input, encoding)
        {
            Endianness = endianness;
        }

        public EndianBinaryReader(Stream input, Encoding encoding, bool leaveOpen, Endianness endianness) : base(input, encoding, leaveOpen)
        {
            Endianness = endianness;
        }

        public override short ReadInt16()
        {
            return ReadInt16(Endianness);
        }

        public override int ReadInt32()
        {
            return ReadInt32(Endianness);
        }

        public override long ReadInt64()
        {
            return ReadInt64(Endianness);
        }

        public override ushort ReadUInt16()
        {
            return ReadUInt16(Endianness);
        }

        public override uint ReadUInt32()
        {
            return ReadUInt32(Endianness);
        }

        public override ulong ReadUInt64()
        {
            return ReadUInt64(Endianness);
        }

        public override float ReadSingle()
        {
            return ReadSingle(Endianness);
        }

        public short ReadInt16(Endianness endianness)
        {
            return BitConverter.ToInt16(ReadForEndianness(sizeof(short), endianness));
        }

        public int ReadInt32(Endianness endianness)
        {
            return BitConverter.ToInt32(ReadForEndianness(sizeof(int), endianness));
        }

        public long ReadInt64(Endianness endianness)
        {
            return BitConverter.ToInt64(ReadForEndianness(sizeof(long), endianness));
        }

        public ushort ReadUInt16(Endianness endianness)
        {
            return BitConverter.ToUInt16(ReadForEndianness(sizeof(ushort), endianness));
        }

        public uint ReadUInt32(Endianness endianness)
        {
            return BitConverter.ToUInt32(ReadForEndianness(sizeof(uint), endianness));
        }

        public ulong ReadUInt64(Endianness endianness)
        {
            return BitConverter.ToUInt64(ReadForEndianness(sizeof(ulong), endianness));
        }

        public float ReadSingle(Endianness endianness)
        {
            return BitConverter.ToSingle(ReadForEndianness(sizeof(float), endianness));
        }

        private byte[] ReadForEndianness(int bytesToRead, Endianness endianness)
        {
            var bytesRead = ReadBytes(bytesToRead);

            if ((endianness == Endianness.Little && !BitConverter.IsLittleEndian)
                || (endianness == Endianness.Big && BitConverter.IsLittleEndian))
            {
                Array.Reverse(bytesRead);
            }

            return bytesRead;
        }
    }
}
