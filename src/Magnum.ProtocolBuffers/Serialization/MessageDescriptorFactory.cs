namespace Magnum.ProtocolBuffers.Serialization
{
    using System;
    using System.Collections.Generic;

    public class MessageDescriptorFactory
    {
        private Dictionary<Type, IMessageDescriptor> _things = new Dictionary<Type, IMessageDescriptor>();

        public IMessageDescriptor<TMessage> Build<TMessage>(MessageMap<TMessage> map) where TMessage : class, new()
        {
            if (_things.ContainsKey(typeof(TMessage)))
                return (IMessageDescriptor<TMessage>)_things[typeof(TMessage)];

            var desc = new MessageDescriptor<TMessage>();

            foreach (var field in map.Fields)
            {
                var wireType = DetermineWireType(field.FieldType);
                var tag = field.NumberTag;
                var readFunc = field.Lambda.Compile();
                var writeFunc = field.Lambda.Compile();

                desc.AddWriter(tag, wireType, writeFunc);
                desc.AddReader(tag, wireType, readFunc);
            }

            _things.Add(typeof(TMessage), desc);
            return desc;
        }

        private WireType DetermineWireType(Type type)
        {
            return WireType.Varint;
        }
    }
}