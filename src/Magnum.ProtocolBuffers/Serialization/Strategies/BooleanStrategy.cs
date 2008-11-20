namespace Magnum.ProtocolBuffers.Serialization.Strategies
{
    using System;

    public class BooleanStrategy :
        ISerializationStrategy
    {
        public bool CanHandle(Type type)
        {
            return typeof (bool).Equals(type);
        }

        public void Serialize(CodedOutputStream stream, int fieldNumber, object value)
        {
            var v = (bool)value;
            ulong s = v ? 1UL : 0UL;
            stream.WriteVarint(fieldNumber, s);
        }

        public object Deserialize(CodedInputStream stream)
        {
            var x = stream.ReadVarint();
            return Convert.ToBoolean(x);
        }
    }
}