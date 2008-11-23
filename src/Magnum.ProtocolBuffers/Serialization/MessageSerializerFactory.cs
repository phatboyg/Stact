
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
    using Common.Reflection;
    using Mapping;
    using Specs;
    using Strategies;

    public class MessageSerializerFactory
    {
        private readonly CommunicationModel _model;

        public MessageSerializerFactory(CommunicationModel model)
        {
            _model = model;
        }

        public ISerializer Build(IMessageDescriptor map)
        {
            if (_model.HasSerializer(map.TypeMapped))
                return _model.GetSerializer(map.TypeMapped);

            IMessageSerializer messageSerializer = new MessageSerializer(map.TypeMapped);

            foreach (var field in map.Fields)
            {
                if(field.Rules.Equals(FieldRules.Repeated))
                {
                    ISerializer repeatedSerializer = null;
                    messageSerializer.AddSubSerializer(repeatedSerializer);
                }
                else if(field.FieldType.Equals(typeof(string)))
                {
                    ISerializer subMessageSerializer = null;
                    messageSerializer.AddSubSerializer(subMessageSerializer);
                }
                else
                {
                    ISerializer defaultSerializer = null;
                    messageSerializer.AddSubSerializer(defaultSerializer);
                }
                //does this need some recursion?
            }

            _model.AddSerializer(messageSerializer);

            return messageSerializer;
        }


        //to be deleted
        public void MessageProperty(ref IMessageSerializer messageSerializer, Mapping.FieldDescriptor field)
        {
            var tag = field.NumberTag;
            var fp = new FastProperty(field.PropertyInfo);
            var netType = field.FieldType;

            messageSerializer.AddProperty(tag, fp, field.FieldType, field.Rules, new MessageStrategy(_model.GetSerializer(netType)));
        }
        public void RepeatableProperty(ref IMessageSerializer messageSerializer, Mapping.FieldDescriptor field)
        {
            var tag = field.NumberTag;
            var fp = new FastProperty(field.PropertyInfo);
            var netType = field.FieldType;
            var repeatedType = netType.GetGenericArguments()[0];
            var serializer = _model.GetFieldSerializer(repeatedType);

            messageSerializer.AddProperty(tag, fp, field.FieldType, field.Rules, new ListStrategy(serializer) );
        }
        public void StandardProperty(ref IMessageSerializer messageSerializer, Mapping.FieldDescriptor field)
        {
            var tag = field.NumberTag;
            var fp = new FastProperty(field.PropertyInfo);
            var netType = field.FieldType;
            messageSerializer.AddProperty(tag, fp, field.FieldType, field.Rules, _model.GetFieldSerializer(netType));
        }
    }
}