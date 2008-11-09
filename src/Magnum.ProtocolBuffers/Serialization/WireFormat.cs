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

    public class WireFormat
    {
        /// <summary>
        /// Number of bits used for storing WireType
        /// </summary>
        private const int TypeBitCount = 3;

        /// <summary>
        /// Mask to remove field number from tag to get type
        /// </summary>
        private const UInt32 TypeBitMask = (1 << TypeBitCount) - 1;

        /// <summary>
        /// Makes a tag for a field
        /// </summary>
        /// <param name="fieldNumber">The assigned number for the field</param>
        /// <param name="type">The wire type for the field</param>
        /// <returns></returns>
        public static UInt32 MakeTag(int fieldNumber, WireType type)
        {
            return (UInt32) ((fieldNumber << TypeBitCount) | (int) type);
        }

        /// <summary>
        /// Returns the WireType given the tag value
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static WireType GetWireType(UInt32 tag)
        {
            return (WireType) (tag & TypeBitMask);
        }

        /// <summary>
        /// Returns the field number given the tag value
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static int GetFieldNumber(UInt32 tag)
        {
            return (int) (tag >> TypeBitCount);
        }
    }
}