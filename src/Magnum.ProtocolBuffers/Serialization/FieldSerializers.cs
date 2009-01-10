namespace Magnum.ProtocolBuffers.Serialization
{
    using System.Collections.Generic;

    public class FieldSerializers
    {
        readonly SortedList<int, FieldSerializer> _serializeProps = new SortedList<int, FieldSerializer>();
        readonly Dictionary<int, FieldSerializer> _deserializeProps = new Dictionary<int, FieldSerializer>();

        public void Add(FieldSerializer serializer)
        {
            _serializeProps.Add(serializer.FieldTag, serializer);
            _deserializeProps.Add(serializer.FieldTag, serializer);
        }


        public IEnumerable<FieldSerializer> GetAll()
        {
            return _serializeProps.Values;
        }

        public FieldSerializer this[int numberTag]
        {
            get { return _deserializeProps[numberTag]; }
        }
    }
}