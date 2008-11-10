namespace Magnum.ProtocolBuffers.Serialization
{
    using System;
    using System.Collections.Generic;

    public class MessageDescriptorFactory
    {
        private Dictionary<Type, MessageDescriptor<string>> _things;

        public MessageDescriptor<TMessage> Build<TMessage>(MessageMap<TMessage> map)
        {
            if (_things.ContainsKey(typeof(TMessage)))
                return _things[typeof (TMessage)];

            var desc = new MessageDescriptor<TMessage>();

            foreach (var field in map.Fields)
            {
                var wireType = DetermineWireType(field.FieldType);
                var tag = field.NumberTag;
                var func = field.Lambda.Compile();

                if(wireType.Equals(WireType.LengthDelimited))
                {
                    Build(null); //field.Map?
                }
            }

            _things.Add(typeof(TMessage), desc);
            return desc;
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