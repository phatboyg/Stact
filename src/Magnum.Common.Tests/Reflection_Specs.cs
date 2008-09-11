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
namespace Magnum.Common.Tests
{
	using System;
	using NUnit.Framework;
	using Reflection;

	[TestFixture]
	public class When_using_a_created_delegate_to_retrieve_an_object_value
	{
		#region Setup/Teardown

		[SetUp]
		public void Setup()
		{
			_created = DateTime.Now;

			_data = new SimpleClass {Name = "Chris", Count = 27, Amount = 123.45m, Created = _created};
		}

		#endregion

		private SimpleClass _data;
		private DateTime _created;

		public class SimpleClass
		{
			public string Name { get; set; }
			public int Count { get; set; }
			public decimal Amount { get; set; }
			public DateTime Created { get; set; }
		}

		[Test]
		public void A_datetime_should_be_returned()
		{
			object value = ReflectionCache<SimpleClass>.Get("Created", _data);
			Assert.IsInstanceOfType(typeof (DateTime), value);
			Assert.AreEqual(_created, value);
		}

		[Test]
		public void A_decimal_should_be_returned()
		{
			object value = ReflectionCache<SimpleClass>.Get("Amount", _data);
			Assert.IsInstanceOfType(typeof (decimal), value);
			Assert.AreEqual(123.45m, value);
		}

		[Test]
		public void A_string_should_be_returned()
		{
			object value = ReflectionCache<SimpleClass>.Get("Name", _data);
			Assert.IsInstanceOfType(typeof (string), value);
			Assert.AreEqual("Chris", value);
		}

		[Test]
		public void An_int_should_be_returned()
		{
			object value = ReflectionCache<SimpleClass>.Get("Count", _data);
			Assert.IsInstanceOfType(typeof (int), value);
			Assert.AreEqual(27, value);
		}
	}
}