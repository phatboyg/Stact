namespace Magnum.Specs.Serialization
{
	using System;
	using System.Diagnostics;
	using Magnum.Extensions;
	using Magnum.Serialization;
	using NUnit.Framework;
	using TestFramework;

	[TestFixture]
	public class Serializing_objects_via_the_fast_serializer
	{
		private Serializer _serializer;

		[SetUp]
		public void Setup()
		{
			_serializer = new FastTextSerializer();
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
			HeftyMessage message = new HeftyMessage
				{
					Int = 47,
					Long = 8675309,
					Dub = 3.14159,
					Flt = 1.234f,
					Boo = true,
					Now = new DateTime(2010,3,1)
				};

			string text = _serializer.Serialize(message);

			text.ShouldEqual("{Int:47,Long:8675309,Dub:3.14159,Flt:1.234,Boo:true,Now:2010-03-01}");
		}

		[Test]
		public void Should_handle_nested_types()
		{
			HeftyMessage message = new HeftyMessage
				{
					Int = 47,
					Long = 8675309,
					Dub = 3.14159,
					Flt = 1.234f,
					Boo = true,
					Now = new DateTime(2010,3,1)
				};

			var parentMessage = new ParentClass {Body = message};

			string text = _serializer.Serialize(parentMessage);

			text.ShouldEqual("{Body:{Int:47,Long:8675309,Dub:3.14159,Flt:1.234,Boo:true,Now:2010-03-01}}");
		}

		[Test, Explicit]
		public void Should_handle_serialization_quickly()
		{
			HeftyMessage message = new HeftyMessage
				{
					Int = 47,
					Long = 8675309,
					Dub = 3.14159,
					Flt = 1.234f,
					Boo = true,
					Now = new DateTime(2010,3,1)
				};

			string text = _serializer.Serialize(message);

			Stopwatch timer = Stopwatch.StartNew();

			var limit = 500000;
			for (int i = 0; i < limit; i++)
			{
				text = _serializer.Serialize(message);
			}

			timer.Stop();

			Trace.WriteLine("elapsed time: " + timer.ElapsedMilliseconds + "ms");
			Trace.WriteLine("messages/sec: " + (limit * 1000) / timer.ElapsedMilliseconds);

		}

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

			public Guid Id { get;set;}
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
