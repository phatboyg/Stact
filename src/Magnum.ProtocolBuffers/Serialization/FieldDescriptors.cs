namespace Magnum.ProtocolBuffers.Serialization
{
    using System.Collections.Generic;

    public class FieldDescriptors
    {
        readonly SortedList<int, FieldDescriptor> _serializeProps = new SortedList<int, FieldDescriptor>();
        readonly Dictionary<int, FieldDescriptor> _deserializeProps = new Dictionary<int, FieldDescriptor>();

        public void Add(FieldDescriptor descriptor)
        {
            _serializeProps.Add(descriptor.FieldTag, descriptor);
            _deserializeProps.Add(descriptor.FieldTag, descriptor);
        }


        public IEnumerable<FieldDescriptor> GetAll()
        {
            return _serializeProps.Values;
        }

        public FieldDescriptor this[int numberTag]
        {
            get { return _deserializeProps[numberTag]; }
        }
    }
}