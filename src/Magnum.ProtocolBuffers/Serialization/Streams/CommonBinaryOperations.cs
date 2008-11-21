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
namespace Magnum.ProtocolBuffers.Serialization.Streams
{
    using System;

    public static class CommonBinaryOperations
    {
        public static bool IsMsbUnset(this byte data)
        {
            return (data & 0x80) == 0x80;
        }

        public static bool IsMsbUnset(this int data)
        {
            return (data & 0x80) == 0x80;
        }

        public static byte RemoveMsb(this byte data)
        {
            return (byte)(data & 0x7f);
        }
        public static UInt64 RemoveMsb(this int data)
        {
            return (byte)(data & 0x7f);
        }
    }
}