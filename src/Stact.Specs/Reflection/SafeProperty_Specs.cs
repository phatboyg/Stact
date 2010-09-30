namespace Stact.Specs.Reflection
{
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using Stact.Reflection;
	using NUnit.Framework;

	[TestFixture]
	public class Settings_a_property_on_a_missing_member
	{
		[Test]
		public void Should_allow_property_to_already_be_set()
		{
			Expression<Func<OuterClass, object>> accessor = o => o.Inner.Value;

			var writer = SafeProperty.Create(accessor);

			const string expected = "Hello";

			var subject = new OuterClass();
			subject.Inner = new InnerClass {OtherValue = "Hi"};

			writer.Set(subject, 0, expected);

			Assert.AreEqual(expected, subject.Inner.Value);
			Assert.AreEqual("Hi", subject.Inner.OtherValue);
		}

		[Test]
		public void Should_create_a_backing_list_for_list_based_properties()
		{
			Expression<Func<OuterClass, object>> accessor = o => o.Inners[0].Value;

			var writer = SafeProperty.Create(accessor);

			const string expected = "Hello";

			var subject = new OuterClass();

			writer.Set(subject, 0, expected);

			Assert.IsNotNull(subject.Inners);
			Assert.AreEqual(expected, subject.Inners[0].Value);
		}

		[Test]
		public void Should_create_a_backing_list_for_list_based_properties_way_deep()
		{
			Expression<Func<WayOuterClass, object>> accessor = o => o.Outer.Inners[0].Value;

			var writer = SafeProperty.Create(accessor);

			const string expected = "Hello";

			var subject = new WayOuterClass();

			writer.Set(subject, 0, expected);

			Assert.IsNotNull(subject.Outer.Inners);
			Assert.AreEqual(expected, subject.Outer.Inners[0].Value);
		}

		[Test]
		public void Should_create_a_list_for_an_indexed_property_value()
		{
			Expression<Func<WayOuterClass, int, object>> accessor = (o,index) => o.Outer.Inners[index].Value;

			var writer = SafeProperty.Create(accessor);

			const string expected = "Hello";

			var subject = new WayOuterClass();

			writer.Set(subject, 0, expected);

			Assert.IsNotNull(subject.Outer.Inners);
			Assert.AreEqual(expected, subject.Outer.Inners[0].Value);
		}

		[Test]
		public void Should_work_for_indexed_properties_way_deep()
		{
			Expression<Func<WayOuterClass, int, object>> accessor = (o,index) => o.Outers[index].Inner.Value;

			var writer = SafeProperty.Create(accessor);

			const string expected = "Hello";

			var subject = new WayOuterClass();

			writer.Set(subject, 1, expected);

			Assert.IsNotNull(subject.Outers);
			Assert.AreEqual(2, subject.Outers.Count);
			Assert.IsNotNull(subject.Outers[1].Inner);
			Assert.AreEqual(expected, subject.Outers[1].Inner.Value);
		}

		[Test]
		public void Should_create_a_list_for_an_indexed_property_value_with_a_non_indexed_property_too()
		{
			Expression<Func<WayOuterClass, int, object>> accessor = (o,index) => o.Outers[index].Inners[0].Value;

			var writer = SafeProperty.Create(accessor);

			const string expected = "Hello";

			var subject = new WayOuterClass();

			writer.Set(subject, 0, expected);

			Assert.IsNotNull(subject.Outers);
			Assert.AreEqual(subject.Outers.Count, 1);
			Assert.AreEqual(expected, subject.Outers[0].Inners[0].Value);
		}

		[Test]
		public void Should_create_a_setter_for_a_simple_value()
		{
			Expression<Func<OuterClass, object>> accessor = o => o.Value;


			var writer = SafeProperty.Create(accessor);


			var subject = new OuterClass();

			const string expected = "Hello";

			writer.Set(subject, 0, expected);

			Assert.AreEqual(expected, subject.Value);
		}

		[Test]
		public void Should_go_way_deep()
		{
			Expression<Func<WayOuterClass, object>> accessor = o => o.Outer.Inner.Value;

			var writer = SafeProperty.Create(accessor);

			const string expected = "Hello";

			var subject = new WayOuterClass();

			writer.Set(subject, 0, expected);

			Assert.AreEqual(expected, subject.Outer.Inner.Value);
		}

		[Test]
		public void Should_not_throw_a_null_reference_exception()
		{
			Expression<Func<OuterClass, object>> accessor = o => o.Inner.Value;

			var writer = SafeProperty.Create(accessor);

			const string expected = "Hello";

			var subject = new OuterClass();

			writer.Set(subject, 0, expected);

			Assert.AreEqual(expected, subject.Inner.Value);
		}

		public class InnerClass
		{
			public string Value { get; set; }
			public string OtherValue { get; set; }
		}

		public class OuterClass
		{
			public InnerClass Inner { get; set; }
			public string Value { get; set; }
			public IList<InnerClass> Inners { get; set; }
		}

		public class WayOuterClass
		{
			public OuterClass Outer { get; private set; }

			public IList<OuterClass> Outers { get; set; }
		}
	}
}