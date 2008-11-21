namespace Magnum.ProtocolBuffers.Serialization
{
    using System.Collections.Generic;

    public class FieldDescriptors<TMessage>
    {
        readonly SortedList<int, FieldDescriptor<TMessage>> _serializeProps = new SortedList<int, FieldDescriptor<TMessage>>();
        readonly Dictionary<int, FieldDescriptor<TMessage>> _deserializeProps = new Dictionary<int, FieldDescriptor<TMessage>>();

        public void Add(FieldDescriptor<TMessage> descriptor)
        {
            _serializeProps.Add(descriptor.FieldTag, descriptor);
            _deserializeProps.Add(descriptor.FieldTag, descriptor);
        }


        public IEnumerable<FieldDescriptor<TMessage>> GetAll()
        {
            return _serializeProps.Values;
        }

        public FieldDescriptor<TMessage> this[int numberTag]
        {
            get { return _deserializeProps[numberTag]; }
        }
    }
}