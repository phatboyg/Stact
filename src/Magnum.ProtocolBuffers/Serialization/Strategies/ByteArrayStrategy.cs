namespace Magnum.ProtocolBuffers.Serialization.Strategies
{
    using System;
    using Streams;

    public class ByteArrayStrategy :
        ISerializationStrategy
    {
        public bool CanHandle(Type type)
        {
            return typeof (Byte[]).Equals(type);
        }

        public void Serialize(CodedOutputStream stream, int fieldNumber, object value)
        {
            throw new System.NotImplementedException();
        }

        public object Deserialize(CodedInputStream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}