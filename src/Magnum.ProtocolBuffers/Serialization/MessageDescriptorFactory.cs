namespace Magnum.ProtocolBuffers.Serialization
{
    using System;

    public class MessageDescriptorFactory
    {
        public MessageDescriptor<TMessage> Build<TMessage>(MessageMap<TMessage> map)
        {
            foreach (var field in map.Fields)
            {
                var wireType = DetermineWireType(field.FieldType);
                if(wireType.Equals(WireType.LengthDelimited))
                {
                    Recurse(field);
                }
                else
                {
                    
                }
                var tag = field.NumberTag;
                var func = field.Lambda.Compile();
            }

            return new MessageDescriptor<TMessage>();
        }

        private void Recurse<TMessage>(FieldMap<TMessage> field)
        {
            
        }

        private WireType DetermineWireType(Type type)
        {
            return WireType.Varint;
        }
    }
}