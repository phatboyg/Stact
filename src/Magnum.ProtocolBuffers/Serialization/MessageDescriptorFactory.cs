namespace Magnum.ProtocolBuffers.Serialization
{
    public class MessageDescriptorFactory
    {
        public MessageDescriptor<TMessage> Build<TMessage>(MessageMap<TMessage> map)
        {
            foreach (var field in map.Fields)
            {
                var tag = field.NumberTag;
                var wireType = field.WireType;
                var func = field.Lambda.Compile();
            }

            return new MessageDescriptor<TMessage>();
        }
    }
}