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
        
        readonly SortedList<int, FieldDescriptor<TMessage>> _serializeProps = new SortedList<int, FieldDescriptor<TMessage>>();
        readonly Dictionary<int, FieldDescriptor<TMessage>> _deserializeProps = new Dictionary<int, FieldDescriptor<TMessage>>();
        static readonly List<ISerializationStrategy> _serializers = new List<ISerializationStrategy>();

        static MessageDescriptor()
        {
            _serializers.Add(new StringSerialization());
            _serializers.Add(new IntSerialization());
            _serializers.Add(new NullableIntSerialization());
            _serializers.Add(new BooleanStrategy());
        }

        public void Serialize(CodedOutputStream outputStream, object message)
        {
            Serialize(outputStream, (TMessage)message);
        }
        object IMessageDescriptor.Deserialize(CodedInputStream inputStream)
        {
            return Deserialize(inputStream);
        }
        public void Serialize(CodedOutputStream outputStream, TMessage message)
        {
            foreach (FieldDescriptor<TMessage> prop in _serializeProps.Values)
            {
                FieldDescriptor<TMessage> prop1 = prop;
                var serializer = _serializers.Find(o => o.CanHandle(prop1.NetType));
                var valueToSerialize = prop.Func.Get(message);
                serializer.Serialize(outputStream, prop1.FieldTag, valueToSerialize);
            }
        }
        public TMessage Deserialize(CodedInputStream inputStream)
        {
            var result = new TMessage();
            var length = inputStream.Length;

            while (inputStream.Position < length)
            {
                TagData tagData = inputStream.ReadTag();
                var field = _deserializeProps[tagData.NumberTag];
                var netType = field.NetType;
                var serializer = _serializers.Find(o => o.CanHandle(netType));
                var deserializedValue = serializer.Deserialize(inputStream);
                field.Func.Set(result, deserializedValue);
            }
            
            return result;
        }

        public bool CanHandle(Type type)
        {
            return typeof (TMessage).Equals(type);
        }

        public Type MessageType
        {
            get
            {
                return typeof (TMessage);
            }
        }

        public void AddProperty(int tag, FastProperty<TMessage> fp, Type netType)
        {
            var fd = new FieldDescriptor<TMessage>
                         {
                             FieldTag = tag,
                             WireType = DetermineWireType(netType),
                             Func = fp,
                             NetType = netType
                         };
            _serializeProps.Add(tag, fd);
            _deserializeProps.Add(tag, fd);
        }
        private static WireType DetermineWireType(Type type)
        {
            if (type.IsEnum)
                return WireType.Varint;

            if (typeof(DateTime).Equals(type)) //uint64
                return WireType.Varint;

            if (typeof(Guid).Equals(type)) //two uint64
                return WireType.LengthDelimited;

            if (typeof(int).Equals(type) || typeof(long).Equals(type))
                return WireType.Varint;


            //string, classes
            return WireType.LengthDelimited;
        }
    }
}