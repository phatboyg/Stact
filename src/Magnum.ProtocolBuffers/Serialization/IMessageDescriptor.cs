namespace Magnum.ProtocolBuffers.Serialization
{
    using System;
    using Common.Reflection;
    using Specs;

    public interface IMessageDescriptor<TMessage> :
        IMessageDescriptor where TMessage : class, new()
    {
        void Serialize(CodedOutputStream outputStream, TMessage message);
        new TMessage Deserialize(CodedInputStream inputStream);
        void AddProperty(int tag, FastProperty<TMessage> fp, Type netType, FieldRules rules);
    }

    public interface IMessageDescriptor
    {
        void Serialize(CodedOutputStream outputStream, object message);
        object Deserialize(CodedInputStream inputStream);
        bool CanHandle(Type type);
        Type MessageType { get; }
    }
}