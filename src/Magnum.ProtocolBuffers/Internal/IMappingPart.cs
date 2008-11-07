namespace Magnum.ProtocolBuffers.Internal
{
    public interface IMappingPart
    {
        void Visit(IMappingVisitor visitor);
    }
}