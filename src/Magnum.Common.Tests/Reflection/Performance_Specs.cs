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
namespace Magnum.Common.Tests.Reflection
{
	using System.Diagnostics;
	using System.IO;
	using System.Runtime.Serialization.Formatters.Binary;
	using NUnit.Framework;

	[TestFixture]
	public class Performance_Specs :
		SpecificationBase
	{
		private BinaryFormatter _formatter = new BinaryFormatter();
		private Customer _data;

		protected override void Before_each_specification()
		{
			_data = new Customer {FirstName = "Chris", LastName = "Patterson", Address = "123 American Way", Age = 27, Amount = 123.45m};
		}

		public class MemoryUsageInfo
		{
			public long BytesUsed { get; set; }
		}

		[Test]
		public void The_performance_of_the_binary_formatter_should_be_slow()
		{
			using (MemoryStream output = new MemoryStream(20000000))
			{
				using (var timer = new FunctionTimer<MemoryUsageInfo>("Custom Serialization", x => Trace.Write(x)))
				{
					for (int j = 0; j < 10; j++)
					{
						output.Seek(0, SeekOrigin.Begin);

						for (int i = 0; i < 100000; i++)
						{
							_formatter.Serialize(output, _data);
						}

						timer.Values.BytesUsed += output.Position;
					}
				}
			}
		}

		[Test]
		public void The_performance_of_this_beast_should_be_fast()
		{
			using (MemoryStream output = new MemoryStream(20000000))
			using (BinaryWriter binaryWriter = new BinaryWriter(output))
			{
				BinarySerializationWriter writer = new BinarySerializationWriter(binaryWriter);

				var serializer = new SerializationUtil<Customer>();

				using (var timer = new FunctionTimer<MemoryUsageInfo>("Custom Serialization", x => Trace.Write(x)))
				{
					for (int j = 0; j < 10; j++)
					{
						output.Seek(0, SeekOrigin.Begin);

						for (int i = 0; i < 100000; i++)
						{
							serializer.Serialize(writer, _data);
						}
						timer.Values.BytesUsed += output.Position;
					}
				}
			}
		}
	}
}