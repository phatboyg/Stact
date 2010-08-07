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
namespace Magnum.Specs.Serialization
{
	using System;
	using System.Diagnostics;
	using Magnum.Serialization;
	using NUnit.Framework;
	using TestFramework;

	[TestFixture]
	public class Serializing_objects_via_the_fast_serializer
	{
		[SetUp]
		public void Setup()
		{
			_serializer = new FastTextSerializer();
		}

		[Test]
		public void Should_handle_nested_types()
		{
			var message = new HeftyMessage
				{
					Int = 47,
					Long = 8675309,
					Dub = 3.14159,
					Flt = 1.234f,
					Boo = true,
					Now = new DateTime(2010, 3, 1)
				};

			var parentMessage = new ParentClass {Body = message};

			string text = _serializer.Serialize(parentMessage);

			text.ShouldEqual("{Body:{Long:8675309,Int:47,Boo:true,Now:2010-03-01,Dub:3.14159,Flt:1.234}}");
		}

		[Test, Explicit]
		public void Should_handle_serialization_quickly()
		{
			var message = new HeftyMessage
				{
					Int = 47,
					Long = 8675309,
					Dub = 3.14159,
					Flt = 1.234f,
					Boo = true,
					Now = new DateTime(2010, 3, 1)
				};

			string text = _serializer.Serialize(message);

			Stopwatch timer = Stopwatch.StartNew();

			int limit = 500000;
			for (int i = 0; i < limit; i++)
			{
				text = _serializer.Serialize(message);
			}

			timer.Stop();

			Trace.WriteLine("elapsed time: " + timer.ElapsedMilliseconds + "ms");
			Trace.WriteLine("messages/sec: " + (limit*1000)/timer.ElapsedMilliseconds);
		}

		[Test]
		public void Should_property_handle_all_types()
		{
			var message = new TestMessage();

			string text = _serializer.Serialize(message);

			text.ShouldEqual("{Id:" + message.Id.ToString("N") + "}");
		}

		[Test]
		public void Should_property_handle_multiple_types()
		{
			var message = new HeftyMessage
				{
					Int = 47,
					Long = 8675309,
					Dub = 3.14159,
					Flt = 1.234f,
					Boo = true,
					Now = new DateTime(2010, 3, 1)
				};

			string text = _serializer.Serialize(message);

			text.ShouldEqual("{Long:8675309,Int:47,Boo:true,Now:2010-03-01,Dub:3.14159,Flt:1.234}");
		}

		private Serializer _serializer;

		private class ParentClass
		{
			public HeftyMessage Body { get; set; }
		}

		private class TestMessage :
			AMessage
		{
			public TestMessage()
			{
				Id = CombGuid.Generate();
			}

			public Guid Id { get; set; }
		}

		private interface AMessage
		{
			Guid Id { get; set; }
		}

		private class HeftyMessage
		{
			public int Int { get; set; }
			public long Long { get; set; }
			public double Dub { get; set; }
			public float Flt { get; set; }
			public bool Boo { get; set; }
			public DateTime Now { get; set; }
		}
	}
}