namespace Magnum.Specs.Extensions
{
	using System.Diagnostics;
	using Magnum.Extensions;
	using NUnit.Framework;
	using System.Collections.Generic;

	[TestFixture]
	public class Using_the_stringify_method_on_an_object
	{
		[Test]
		public void Should_create_a_nice_output_for_a_list()
		{
			var items = new List<string>
				{
					"One", "Two", "Three"
				};

			Trace.WriteLine(items.Stringify());
		}

		[Test]
		public void Should_create_a_property_output_for_a_class()
		{
			var subject = new Subject() 
			{
				Name = "Chris", Count = 27,
				Items = new List<int>() { 1,2,3,4,5,6 }
			};

			Trace.WriteLine(subject.Stringify());
		}

		private class Subject
		{
			public string Name { get; set; }
			public int Count { get; set; }
			public IList<int> Items { get; set; }
		}
	}
}
