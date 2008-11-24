namespace Magnum.ProtocolBuffers.Serialization
{
    using System;
    using Streams;

    public interface IMessageSerializer : ISerializer
    {
        void AddField(FieldSerializer fieldSerializer);

    }

    public interface ISerializer
    {
        void Serialize(CodedOutputStream outputStream, object instance);
        object Deserialize(CodedInputStream inputStream);

        //void Serialize<TValue>(CodedOutputStream outputStream, TValue value);
        //TMessage Deserialize<TMessage>(CodedInputStream inputStream) where TMessage : class, new();

        bool CanHandle(Type type);
    }
}