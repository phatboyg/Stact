namespace Magnum.ProtocolBuffers
{
    using System;
    using Mapping;

    public interface IMap
    {
        Type TypeMapped { get; }
    }

    public interface IMap<TMessage> :
        IMap
    {
        MessageDescriptor<TMessage> GetDescriptor();
    }
}