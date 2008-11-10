namespace Magnum.ProtocolBuffers.Serialization
{
    using System;

    public class MessageDescriptorFactory
    {
        public MessageDescriptor<TMessage> Build<TMessage>(MessageMap<TMessage> map)
        {
            foreach (var field in map.Fields)
            {
                var tag = field.NumberTag;
                var wireType = DetermineWireType(field.FieldType);
                var func = field.Lambda.Compile();
            }

            return new MessageDescriptor<TMessage>();
        }

        private WireType DetermineWireType(Type type)
        {
            return WireType.Varint;
        }
    }
}