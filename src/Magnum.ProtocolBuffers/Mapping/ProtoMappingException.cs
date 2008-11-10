namespace Magnum.ProtocolBuffers
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class ProtoMappingException : Exception
    {
        public ProtoMappingException()
        {
        }

        public ProtoMappingException(string message) : base(message)
        {
        }

        public ProtoMappingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ProtoMappingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}