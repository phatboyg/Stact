namespace Magnum.ProtocolBuffers.Internal
{
    public interface IMapping
    {
        void Visit(IMappingVisitor visitor);
    }
}