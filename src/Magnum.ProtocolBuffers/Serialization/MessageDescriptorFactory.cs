namespace Magnum.ProtocolBuffers.Serialization
{
    using System;
    using System.Collections.Generic;
    using Common.Reflection;

    public class MessageDescriptorFactory
    {
        private readonly Dictionary<Type, IMessageDescriptor> _things = new Dictionary<Type, IMessageDescriptor>();

        public IMessageDescriptor<TMessage> Build<TMessage>(MessageMap<TMessage> map) where TMessage : class, new()
        {
            if (_things.ContainsKey(typeof(TMessage)))
                return (IMessageDescriptor<TMessage>)_things[typeof(TMessage)];

            var desc = new MessageDescriptor<TMessage>();

            foreach (var field in map.Fields)
            {
                var tag = field.NumberTag;
                var fp = new FastProperty<TMessage>(field.PropertyInfo);

                desc.AddProperty(tag, fp, field.FieldType);
            }

            _things.Add(typeof(TMessage), desc);
            return desc;
        }

    }
}