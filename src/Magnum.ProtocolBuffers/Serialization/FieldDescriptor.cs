namespace Magnum.ProtocolBuffers.Serialization
{
    using Common.Reflection;

    public class FieldDescriptor<TMessage>
    {
        public int FieldTag { get; set; }
        public FastProperty<TMessage> Func { get; set; }
        public WireType WireType { get; set; }
    }
}