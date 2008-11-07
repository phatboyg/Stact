namespace Magnum.ProtocolBuffers.Internal
{
    using System;

    public interface IMappingVisitor
    {
        Type CurrentType { get; set; }
        void AddMap(string contentToAppend);
        void AddSerializer(Type typeMapped, object serializer);
    }
}