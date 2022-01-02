using System;
using System.IO;
using System.Text;

namespace FSGSave
{
    public class EndianBinaryWriter : BinaryWriter
    {
        public Endianness Endianness { get; }

        public EndianBinaryWriter(Stream input) : this(input, Endianness.Little) { }

        public EndianBinaryWriter(Stream input, Encoding encoding) : this(input, encoding, Endianness.Little) { }

        public EndianBinaryWriter(Stream input, Encoding encoding, bool leaveOpen)
            : this(input, encoding, leaveOpen, Endianness.Little) { }

        public EndianBinaryWriter(Stream input, Endianness endianness) : base(input)
        {
            Endianness = endianness;
        }

        public EndianBinaryWriter(Stream input, Encoding encoding, Endianness endianness) : base(input, encoding)
        {
            Endianness = endianness;
        }

        public EndianBinaryWriter(Stream input, Encoding encoding, bool leaveOpen, Endianness endianness) : base(input, encoding, leaveOpen)
        {
            Endianness = endianness;
        }

        public override void Write(short value)
        {
            Write(value, Endianness);
        }

        public override void Write(int value)
        {
            Write(value, Endianness);
        }

        public override void Write(long value)
        {
            Write(value, Endianness);
        }

        public override void Write(ushort value)
        {
            Write(value, Endianness);
        }

        public override void Write(uint value)
        {
            Write(value, Endianness);
        }

        public override void Write(ulong value)
        {
            Write(value, Endianness);
        }

        public override void Write(float value)
        {
            Write(value, Endianness);
        }

        public void Write(short value, Endianness endianness)
        {
            Write(BitConverter.GetBytes(value), endianness);
        }

        public void Write(int value, Endianness endianness)
        {
            Write(BitConverter.GetBytes(value), endianness);
        }

        public void Write(long value, Endianness endianness)
        {
            Write(BitConverter.GetBytes(value), endianness);
        }

        public void Write(ushort value, Endianness endianness)
        {
            Write(BitConverter.GetBytes(value), endianness);
        }

        public void Write(uint value, Endianness endianness)
        {
            Write(BitConverter.GetBytes(value), endianness);
        }

        public void Write(ulong value, Endianness endianness)
        {
            Write(BitConverter.GetBytes(value), endianness);
        }

        public void Write(float value, Endianness endianness)
        {
            Write(BitConverter.GetBytes(value), endianness);
        }

        private void Write(byte[] buffer, Endianness endianness)
        {
            if ((endianness == Endianness.Little && !BitConverter.IsLittleEndian)
                || (endianness == Endianness.Big && BitConverter.IsLittleEndian))
            {
                Array.Reverse(buffer);
            }

            Write(buffer);
        }
    }
}
