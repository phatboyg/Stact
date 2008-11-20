namespace Magnum.ProtocolBuffers.Internal
{
    using System;
    using System.Collections.Generic;
    using Common;

    public interface IMap
    {
        string Name { get; }
        Type TypeMapped { get; }
        Range<int> ExtensionRange { get; }
        void Visit(IMappingVisitor visitor);
        
    }

    public interface IMap<TMessage> :
        IMap
    {
        IList<FieldMap<TMessage>> Fields { get; }
    }
}