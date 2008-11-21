namespace Magnum.ProtocolBuffers.Serialization.Strategies
{
    using System;

    public class MessageSerialization :
        ISerializationStrategy
    {
        private IMessageSerializer _serializer;

        public MessageSerialization(IMessageSerializer serializer)
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