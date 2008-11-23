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
    using Streams;

    public class NullableSignedInt64Strategy :
        ISerializationStrategy
    {
        public bool CanHandle(Type type)
        {
            return typeof(Int64?).Equals(type);
        }

        public void Serialize(CodedOutputStream stream, int fieldNumber, object value)
        {
            Int64? v = (Int64?) value;
            if(v.HasValue)
                stream.WriteVarint(fieldNumber, (uint)v.Value);
        }

        public object Deserialize(CodedInputStream stream)
        {
            return (Int64?)stream.ReadVarint();
        }

        public WireType WireType
        {
            get { return WireType.Varint; }
        }
    }
}