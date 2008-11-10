namespace Magnum.ProtocolBuffers.Serialization
{
    public interface IMessageDescriptor<TMessage>
    {
        void Serialize(CodedOutputStream outputStream, TMessage message);
        TMessage Deserialize(CodedInputStream inputStream);
    }

    public interface IMessageDescriptor
    {
        void Serialize(CodedOutputStream outputStream, object message);
        object Deserialize(CodedInputStream inputStream);
    }
}