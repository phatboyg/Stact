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
namespace Magnum.Common.Serialization
{
	using System;
	using System.IO;

	public class BinarySerializationReader :
		ISerializationReader
	{
		private readonly BinaryReader _reader;

		public BinarySerializationReader(BinaryReader reader)
		{
			_reader = reader;
		}

		public string ReadString()
		{
			return _reader.ReadString();
		}

		public int ReadInt32()
		{
			return _reader.ReadInt32();
		}

		public decimal ReadDecimal()
		{
			return _reader.ReadDecimal();
		}

		public long ReadInt64()
		{
			return _reader.ReadInt64();
		}

		public double ReadDouble()
		{
			return _reader.ReadDouble();
		}

		public DateTime ReadDateTime()
		{
			return DateTime.FromBinary(_reader.ReadInt64());
		}

		public bool ReadBoolean()
		{
			return _reader.ReadBoolean();
		}

		public float ReadSingle()
		{
			return _reader.ReadSingle();
		}

		public short ReadInt16()
		{
			return _reader.ReadInt16();
		}

		public ushort ReadUInt16()
		{
			return _reader.ReadUInt16();
		}

		public ulong ReadUInt64()
		{
			return _reader.ReadUInt64();
		}

		public uint ReadUInt32()
		{
			return _reader.ReadUInt32();
		}
	}
}