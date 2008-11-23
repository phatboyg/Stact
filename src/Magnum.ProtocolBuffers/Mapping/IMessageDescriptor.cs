namespace Magnum.ProtocolBuffers.Mapping
{
    using System;
    using System.Collections.Generic;

    public interface IMessageDescriptor
    {
        Type TypeMapped { get; }

        IList<FieldDescriptor> Fields { get; }
    }
}