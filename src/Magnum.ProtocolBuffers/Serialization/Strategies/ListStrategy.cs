namespace Magnum.ProtocolBuffers.Serialization.Strategies
{
    using System;
    using System.Collections;

    public class ListStrategy :
        ISerializationStrategy
    {
        public bool CanHandle(Type type)
        {
            return typeof (IEnumerable).IsAssignableFrom(type);
        }

        public void Serialize(CodedOutputStream stream, int fieldNumber, object value)
        {
            stream.WriteTag(fieldNumber, WireType.Varint);
            IEnumerable repeatedStuff = (IEnumerable) value;

            foreach (var o in repeatedStuff)
            {
                //get a serilaization strategy for type of o
            }
        }

        public object Deserialize(CodedInputStream stream)
        {
            //this one is a bit odd
            //need to get the deserialization strategy
            //and call it
            return stream.ReadString();
        }
    }
}