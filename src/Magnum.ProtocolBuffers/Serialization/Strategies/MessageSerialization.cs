namespace Magnum.ProtocolBuffers.Serialization.Strategies
{
    using System;

    public class MessageSerialization :
        ISerializationStrategy
    {
        private IMessageDescriptor _descriptor;

        public MessageSerialization(IMessageDescriptor descriptor)
        {
            _descriptor = descriptor;
        }

        public bool CanHandle(Type type)
        {
            return _descriptor.CanHandle(type);
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