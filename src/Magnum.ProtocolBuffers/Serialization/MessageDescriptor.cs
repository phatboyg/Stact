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
    using Strategies;

    public class MessageDescriptor<TMessage> :
        IMessageDescriptor<TMessage> where TMessage : class, new()
    {
        readonly List<FieldDescriptor<TMessage>> _serializeProps = new List<FieldDescriptor<TMessage>>();
        readonly Dictionary<int, FieldDescriptor<TMessage>> _deserializeProps = new Dictionary<int, FieldDescriptor<TMessage>>();
        private readonly List<ISerializationStrategy> _serializers = new List<ISerializationStrategy>();

        public MessageDescriptor()
        {
            _serializers.Add(new StringSerialization());
            _serializers.Add(new IntSerialization());
            _serializers.Add(new NullableIntSerialization());
        }

        public void Serialize(CodedOutputStream outputStream, object message)
        {
            throw new System.NotImplementedException();
        }

        object IMessageDescriptor.Deserialize(CodedInputStream inputStream)
        {
            return Deserialize(inputStream);
        }

        public void Serialize(CodedOutputStream outputStream, TMessage message)
        {
            foreach (FieldDescriptor<TMessage> prop in _serializeProps)
            {
                FieldDescriptor<TMessage> prop1 = prop;
                var s = _serializers.Find((o) => o.CanHandle(prop1.NetType));
                var value = prop.Func.Get(message);
                s.Serialize(outputStream, prop1.FieldTag, value);
            }
        }

        public TMessage Deserialize(CodedInputStream inputStream)
        {
            TMessage result = new TMessage();
            var length = inputStream.Length;
            while (inputStream.Position < length)
            {
                TagData data = inputStream.ReadTag();
                var netType = _deserializeProps[data.NumberTag].NetType;
                var s = _serializers.Find(o => o.CanHandle(netType));
                var value = s.Deserialize(inputStream);
                _deserializeProps[data.NumberTag].Func.Set(result, value);
            }
            return result;
        }

        public void AddProperty(int tag, WireType type, FastProperty<TMessage> fp, Type netType)
        {
            var fd = new FieldDescriptor<TMessage>()
                         {
                             FieldTag = tag,
                             WireType = type,
                             Func = fp,
                             NetType = netType
                         };
            _serializeProps.Add(fd);
            _deserializeProps.Add(tag, fd);
        }
    }
}