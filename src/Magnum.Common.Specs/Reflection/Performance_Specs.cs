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
namespace Magnum.Common.Specs.Reflection
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Runtime.Serialization.Formatters.Binary;
	using NUnit.Framework;
	using Serialization;

	[TestFixture]
	public class When_serializing_an_object :
		SpecificationBase
	{
		private readonly BinaryFormatter _formatter = new BinaryFormatter();
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
				using (var timer = new FunctionTimer<MemoryUsageInfo>("Binary Formatter", x => Trace.WriteLine(x)))
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

				using (var timer = new FunctionTimer<MemoryUsageInfo>("Custom Serialization", x => Trace.WriteLine(x)))
				{
					for (int j = 0; j < 10; j++)
					{
						output.Seek(0, SeekOrigin.Begin);

						for (int i = 0; i < 100000; i++)
						{
							SerializationUtil<Customer>.Serialize(writer, _data);
						}
						timer.Values.BytesUsed += output.Position;
					}
				}
			}
		}
	}

	[TestFixture]
	public class When_deserializing_an_object :
		SpecificationBase
	{
		private readonly BinaryFormatter _formatter = new BinaryFormatter();
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
			byte[] bytes;
			using (MemoryStream storage = new MemoryStream())
			{
				_formatter.Serialize(storage, _data);
				storage.Flush();
				bytes = storage.ToArray();

				storage.Close();
			}

			using (MemoryStream output = new MemoryStream(bytes))
			{
				using (var timer = new FunctionTimer<MemoryUsageInfo>("BinaryFormatter Deserialization", x => Trace.WriteLine(x)))
				{
					for (int i = 0; i < 300000; i++)
					{
						output.Seek(0, SeekOrigin.Begin);

						Customer data = (Customer) _formatter.Deserialize(output);

						Assert.AreEqual(data.FirstName, _data.FirstName);
						Assert.AreEqual(data.LastName, _data.LastName);
						Assert.AreEqual(data.Address, _data.Address);
					}
				}
			}
		}

		[Test]
		public void The_performance_of_this_beast_should_be_fast()
		{
			byte[] bytes;
			using (MemoryStream storage = new MemoryStream())
			using (BinaryWriter binaryWriter = new BinaryWriter(storage))
			{
				BinarySerializationWriter writer = new BinarySerializationWriter(binaryWriter);

				SerializationUtil<Customer>.Serialize(writer, _data);

				binaryWriter.Flush();
				binaryWriter.Close();

				storage.Flush();
				bytes = storage.ToArray();

				storage.Close();
			}

			using (MemoryStream output = new MemoryStream(bytes))
			using (BinaryReader binaryReader = new BinaryReader(output))
			{
				BinarySerializationReader writer = new BinarySerializationReader(binaryReader);

				using (var timer = new FunctionTimer<MemoryUsageInfo>("Custom Deserialization", x => Trace.WriteLine(x)))
				{
					for (int i = 0; i < 300000; i++)
					{
						output.Seek(0, SeekOrigin.Begin);

						Customer data = SerializationUtil<Customer>.Deserialize(writer);

						Assert.AreEqual(data.FirstName, _data.FirstName);
						Assert.AreEqual(data.LastName, _data.LastName);
						Assert.AreEqual(data.Address, _data.Address);
					}
				}
			}
		}
	}

	[TestFixture]
	public class When_creating_an_object_in_the_CLR
	{
		[Test]
		public void Using_expressions_should_be_much_faster()
		{
			Type type = typeof (Customer);

			ConstructorInfo ci = type.GetConstructor(new Type[] {});

			Func<Customer> ctor = Expression.Lambda<Func<Customer>>(Expression.New(ci)).Compile();

			ctor();

			using (FunctionTimer t = new FunctionTimer("Using the expression syntax", x => Trace.WriteLine(x)))
			{
				for (int i = 0; i < 10000000; i++)
				{
					Customer c = ctor();
				}
			}
		}

		[Test]
		public void Using_the_default_constructor_info()
		{
			Type type = typeof (Customer);

			ConstructorInfo ci = type.GetConstructor(new Type[] {});
			ci.Invoke(new object[] {});

			using (FunctionTimer t = new FunctionTimer("Constructor Info (Invoke)", x => Trace.WriteLine(x)))
			{
				for (int i = 0; i < 10000000; i++)
				{
					Customer c = (Customer) ci.Invoke(null);
				}
			}
		}
	}
}