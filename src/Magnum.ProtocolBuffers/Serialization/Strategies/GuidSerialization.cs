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
namespace Magnum.ProtocolBuffers.Serialization.Strategies
{
    using System;
    using System.Text;

    public class GuidSerialization :
        ISerializationStrategy
    {
        public bool CanHandle(Type type)
        {
            return typeof(Guid).Equals(type);
        }

        public void Serialize(CodedOutputStream stream, int fieldNumber, object value)
        {
            var valueToSerialize = (Guid) value;

            var binaryData = valueToSerialize.ToByteArray();
            stream.WriteString(fieldNumber, Encoding.UTF8.GetString(binaryData));
        }

        public object Deserialize(CodedInputStream stream)
        {
            string binaryData = stream.ReadString();
            return new Guid(Encoding.UTF8.GetBytes(binaryData));
        }
    }
}