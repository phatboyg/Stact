namespace Magnum.ProtocolBuffers.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class CodedInputStream :
        IDisposable
    {
        private volatile bool _disposed;
        private MemoryStream _stream;

        public CodedInputStream(byte[] input)
        {
            _stream = new MemoryStream(input);
        }

        public int Length
        {
            get { return (int)_stream.Length; }
        }


        public TagData ReadTag()
        {
            var tagData = (byte)_stream.ReadByte();

            return new TagData
                       {
                           NumberTag = WireFormat.GetFieldNumber(tagData),
                           WireType = WireFormat.GetWireType(tagData)
                       };
        }

        public Int32 ReadFixedInt32()
        {
            Int32 working = _stream.ReadByte();
            working |= _stream.ReadByte() << 8;
            working |= _stream.ReadByte() << 16;
            working |= _stream.ReadByte() << 24;

            return working;
        }
        public Int64 ReadFixedInt64()
        {
            Int64 working = _stream.ReadByte();
            working |= ((Int64)_stream.ReadByte()) << 8;
            working |= ((Int64)_stream.ReadByte()) << 16;
            working |= ((Int64)_stream.ReadByte()) << 24;
            working |= ((Int64)_stream.ReadByte()) << 32;
            working |= ((Int64)_stream.ReadByte()) << 40;
            working |= ((Int64)_stream.ReadByte()) << 48;
            working |= ((Int64)_stream.ReadByte()) << 56;

            return working;
        }
        public UInt64 ReadVarint()
        {
            int offset = 0;
            int b;
            UInt64 value = 0;

            while ((b = _stream.ReadByte()) >= 0)
            {
                value |= b.RemoveMsb().Shift(offset);
                offset += 7;
            }
            return value;
        }
        public string ReadString()
        {
            int length = _stream.ReadByte();
            var stringData = new byte[length];
            _stream.Read(stringData, 0, length);
            return Encoding.UTF8.GetString(stringData);
        }


        private byte[] CollectAndReverseBytes(MemoryStream stream)
        {
            bool foundMSB = false;
            Stack<byte> bytes = new Stack<byte>();
            while (!foundMSB)
            {
                byte current = (byte) stream.ReadByte();

                if (current.IsMsbUnset())
                    foundMSB = true;
                
                bytes.Push(current);
            }
            return bytes.ToArray();
        }

        private int CalculateNumber(byte[] bytes)
        {
            return 0;
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed) return;

            if (_stream != null)
                _stream.Dispose();

            _stream = null;
            _disposed = true;
        }

        ~CodedInputStream()
        {
            Dispose(false);
        }

        
    }
}