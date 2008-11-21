namespace Magnum.ProtocolBuffers.Mapping
{
    using System;
    using System.Collections.Generic;

    public interface IMessageDescriptor
    {
        Type TypeMapped { get; }
    }

    public interface IMessageDescriptor<TMessage> :
        IMessageDescriptor
    {
        IList<FieldDescriptor<TMessage>> Fields { get; }
    }
}