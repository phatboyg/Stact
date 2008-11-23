namespace Magnum.ProtocolBuffers.Serialization
{
    using System;
    using Common.Reflection;
    using Specs;
    using Streams;

    public interface IMessageSerializer : ISerializer
    {

        
        void AddProperty(int tag, FastProperty fp, Type netType, FieldRules rules, ISerializationStrategy strategy);
    }

    public interface ISerializer
    {
        void Serialize(CodedOutputStream outputStream, object message);
        void Serialize<TMessage>(CodedOutputStream outputStream, TMessage message) where TMessage : class;
        object Deserialize(CodedInputStream inputStream);
        TMessage Deserialize<TMessage>(CodedInputStream inputStream) where TMessage : class, new();

        bool CanHandle(Type type);

        void AddSubSerializer(ISerializer serializer);
    }
}