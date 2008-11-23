namespace Magnum.ProtocolBuffers.Serialization.Strategies
{
    using System;
    using Streams;

    public class MessageStrategy :
        ISerializationStrategy
    {
        private ISerializer _serializer;

        public MessageStrategy(ISerializer serializer)
        {
            _serializer = serializer;
        }

        public bool CanHandle(Type type)
        {
            return _serializer.CanHandle(type);
        }

        public void Serialize(CodedOutputStream stream, int fieldNumber, object value)
        {
            
        }

        public object Deserialize(CodedInputStream stream)
        {
            return null;
        }
    }
}