namespace Magnum.ProtocolBuffers.Serialization
{
    using System;
    using System.IO;

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
                int i = dataBuffer.Length;
            }
            
            return tag;
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