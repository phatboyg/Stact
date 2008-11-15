namespace Magnum.ProtocolBuffers.Serialization.Strategies
{
    using System;

    public class DateTimeSerialization :
        ISerializationStrategy
    {
        public bool CanHandle(Type type)
        {
            return typeof (DateTime).Equals(type);
        }

        public void Serialize(CodedOutputStream stream, int fieldNumber, object value)
        {
            var valueToSerialize = (DateTime) value;

            long binaryDate = valueToSerialize.ToBinary();
            stream.WriteVarint(fieldNumber, (uint)binaryDate);
        }

        public object Deserialize(CodedInputStream stream)
        {
            ulong binaryDate = stream.ReadVarint();
            return DateTime.FromBinary((long)binaryDate);
        }
    }
}