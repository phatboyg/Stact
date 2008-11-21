namespace Magnum.ProtocolBuffers.Serialization
{
    using System;
    using Common.Reflection;
    using Specs;
    using Streams;

    public interface IMessageSerializer<TMessage> :
        IMessageSerializer where TMessage : class, new()
    {
        void Serialize(CodedOutputStream outputStream, TMessage message);
        new TMessage Deserialize(CodedInputStream inputStream);
    }

    public interface IMessageSerializer
    {
        void AddProperty(int tag, FastProperty fp, Type netType, FieldRules rules, ISerializationStrategy strategy);
        void Serialize(CodedOutputStream outputStream, object message);
        object Deserialize(CodedInputStream inputStream);
        bool CanHandle(Type type);
        Type MappedType { get; }
    }
}