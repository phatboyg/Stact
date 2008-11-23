namespace Magnum.ProtocolBuffers.Serialization.Strategies
{
    using System;
    using Streams;

    public class UnsignedInt32Strategy :
        ISerializationStrategy
    {
        public bool CanHandle(Type type)
        {
            return typeof(UInt32).Equals(type);
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