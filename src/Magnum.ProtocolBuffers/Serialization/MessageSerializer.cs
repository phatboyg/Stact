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
    using Streams;

    public class MessageSerializer :
        IMessageSerializer
    {
        private readonly FieldDescriptors _descriptors = new FieldDescriptors();

        public MessageSerializer(Type mappedType)
        {
            MappedType = mappedType;
        }


        public void Serialize(CodedOutputStream outputStream, object value)
        {
            foreach (FieldSerializer prop in _descriptors.GetAll())
            {
                var valueToSerialize = prop.GetFieldValue(value);
                prop.Strategy.Serialize(outputStream, prop.FieldTag, valueToSerialize);
            }
        }
        //public void Serialize<TMessage>(CodedOutputStream outputStream, TMessage message)
        //{
        //    Serialize(outputStream, (object)message);
        //}

        public object Deserialize(CodedInputStream inputStream)
        {
            object result = Activator.CreateInstance(MappedType);
            var length = inputStream.Length;

            while (inputStream.Position < length)
            {
                TagData tagData = inputStream.ReadTag();
                var field = _descriptors[tagData.NumberTag];
                var deserializedValue = field.Strategy.Deserialize(inputStream);
                field.SetField(result, deserializedValue);
            }

            return result;
        }

        //public TMessage Deserialize<TMessage>(CodedInputStream inputStream) where TMessage : class, new()
        //{
        //    var result = new TMessage();
        //    var length = inputStream.Length;

        //    while (inputStream.Position < length)
        //    {
        //        TagData tagData = inputStream.ReadTag();
        //        var field = _descriptors[tagData.NumberTag];
        //        var deserializedValue = field.Strategy.Deserialize(inputStream);
        //        field.Func.Set(result, deserializedValue);
        //    }
            
        //    return result;
        //}

        public bool CanHandle(Type type)
        {
            return MappedType.Equals(type);
        }
        public Type MappedType
        {
            get; private set;
        }
        public void AddField(FieldSerializer fd)
        {
            _descriptors.Add(fd);
        }
    }
}