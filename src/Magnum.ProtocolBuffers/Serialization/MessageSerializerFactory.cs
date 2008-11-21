
// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace Magnum.ProtocolBuffers.Serialization
{
    using System;
    using System.Collections.Generic;
    using Common.Reflection;
    using Mapping;
    using Strategies;

    public class MessageSerializerFactory
    {
        private readonly Dictionary<Type, IMessageSerializer> _descriptors = new Dictionary<Type, IMessageSerializer>();
        readonly List<ISerializationStrategy> _serializers = new List<ISerializationStrategy>();

        public MessageSerializerFactory()
        {

            _serializers.Add(new StringStrategy());
            _serializers.Add(new IntStrategy());
            _serializers.Add(new NullableIntStrategy());
            _serializers.Add(new BooleanStrategy());
        }
        public IMessageSerializer Build<TMessage>(IMessageDescriptor<TMessage> map) where TMessage : class, new()
        {
            if (_descriptors.ContainsKey(typeof(TMessage)))
                return _descriptors[typeof(TMessage)];

            IMessageSerializer<TMessage> messageSerializer = new MessageSerializer<TMessage>();

            foreach (var field in map.Fields)
            {
                //if repeated
                //use repeated strategy

                //if message
                //use message strategy
                //add to serializers

                //else use non-repeated

                StandardProperty(ref messageSerializer, field);


                //does this need some recursion?
            }

            _descriptors.Add(typeof(TMessage), messageSerializer);
            return messageSerializer;
        }

        public void MessageProperty<TMessage>(ref IMessageSerializer<TMessage> messageSerializer, Mapping.FieldDescriptor<TMessage> field) where TMessage : class, new()
        {
            var tag = field.NumberTag;
            var fp = new FastProperty(field.PropertyInfo);
            var netType = field.FieldType;

            messageSerializer.AddProperty(tag, fp, field.FieldType, field.Rules, new MessageStrategy(_descriptors[netType]));
        }

        public void RepeatableProperty<TMessage>(ref IMessageSerializer<TMessage> messageSerializer, Mapping.FieldDescriptor<TMessage> field) where TMessage : class, new()
        {
            var tag = field.NumberTag;
            var fp = new FastProperty(field.PropertyInfo);
            var netType = field.FieldType;
            var repeatedType = netType.GetGenericArguments()[0];
            var serializer = _serializers.Find(x => x.CanHandle(repeatedType));

            messageSerializer.AddProperty(tag, fp, field.FieldType, field.Rules, new ListStrategy(serializer) );
        }

        public void StandardProperty<TMessage>(ref IMessageSerializer<TMessage> messageSerializer, Mapping.FieldDescriptor<TMessage> field) where TMessage : class, new()
        {
            var tag = field.NumberTag;
            var fp = new FastProperty(field.PropertyInfo);
            var netType = field.FieldType;
            messageSerializer.AddProperty(tag, fp, field.FieldType, field.Rules, _serializers.Find(x => x.CanHandle(netType)));
        }
    }

    public class FieldSerializer
    {
        
    }

    public class RepeatedFieldSerializer
    {
        
    }

    //this is the last one to be added?
    public class MessageFieldSerializer
    {
      
    }
}