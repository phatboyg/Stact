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



        public object ReadNextMessage()
        {
            //the first byte contains the tag and type
            byte tag = (byte)_stream.ReadByte();
            var fieldNumber = WireFormat.GetFieldNumber(tag);
            var wireType = WireFormat.GetWireType(tag);

            if(wireType.Equals(WireType.LengthDelimited))
            {
                int length = _stream.ReadByte();
                byte[] dataBuffer = new byte[length];
                _stream.Read(dataBuffer, 0, length);
                return Encoding.UTF8.GetString(dataBuffer);
            }

            if(wireType.Equals(WireType.Varint))
            {
                int offset = 0;
                int b;
                UInt64 value = 0;

                while((b = _stream.ReadByte()) >= 0)
                {
                    value |= b.RemoveMsb().Shift(offset);
                    offset += 7;
                }
                return value;
            }

            return tag;
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