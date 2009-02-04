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
namespace Magnum.Common.Specs
{
	using System.IO;
	using Magnum.Specs;
	using Serialization;

	public class SerializationSpecificationBase :
		SpecificationBase
	{
		public byte[] Serialize<T>(T obj) where T : new()
		{
			byte[] bytes;
			using (var storage = new MemoryStream())
			using (var binaryWriter = new BinaryWriter(storage))
			{
				var writer = new BinarySerializationWriter(binaryWriter);

				SerializationUtil<T>.Serialize(writer, obj);

				binaryWriter.Flush();
				binaryWriter.Close();

				storage.Flush();
				bytes = storage.ToArray();

				storage.Close();
			}
			return bytes;
		}

		public T Deserialize<T>(byte[] bytes) where T : class, new()
		{
			T obj = null;

			using (var output = new MemoryStream(bytes))
			using (var binaryReader = new BinaryReader(output))
			{
				var writer = new BinarySerializationReader(binaryReader);
				output.Seek(0, SeekOrigin.Begin);

				obj = SerializationUtil<T>.Deserialize(writer);
			}

			return obj;
		}
	}
}