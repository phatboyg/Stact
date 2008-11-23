
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
    using System.Collections.Generic;
    using Common.Reflection;
    using Mapping;
    using Specs;
    using Strategies;

    public class MessageSerializerFactory
    {
        private readonly CommunicationModel _model;
        private readonly Dictionary<FieldRules, object> _strategies = new Dictionary<FieldRules, object>();

        public MessageSerializerFactory(CommunicationModel model)
        {
            _model = model;
            _strategies.Add(FieldRules.Repeated, null);
            _strategies.Add(FieldRules.Optional, null);
            _strategies.Add(FieldRules.Required, null);
        }

        public ISerializer Build(IMessageDescriptor map)
        {
            if (_model.HasSerializer(map.TypeMapped))
                return _model.GetSerializer(map.TypeMapped);

            IMessageSerializer messageSerializer = new MessageSerializer(map.TypeMapped);

            foreach (var field in map.Fields)
            {
                messageSerializer.AddProperty(field.NumberTag, null, field.NetType, field.Rules, _model.GetFieldSerializer(field.NetType));
            }

            _model.AddMessageSerializer(messageSerializer);

            return messageSerializer;
        }
    }
}