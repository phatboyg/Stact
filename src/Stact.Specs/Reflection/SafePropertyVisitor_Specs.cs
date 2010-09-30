namespace Stact.Specs.Reflection
{
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using Stact.Reflection;
	using NUnit.Framework;

	[TestFixture]
	public class Using_the_safe_property_setter
	{
		[Test]
		public void Should_set_first_level_property()
		{
			Expression<Func<Outer, string>> expression = o => o.Value;
			SafeProperty setter = SafeProperty.Create(expression);

			var outer = new Outer();
			setter.Set(outer, 0, "Hello");

			Assert.AreEqual("Hello", outer.Value);
		}

		[Test]
		public void Should_set_second_level_property()
		{
			Expression<Func<Outer, string>> expression = o => o.Middle.Value;
			SafeProperty setter = SafeProperty.Create(expression);

			var outer = new Outer();
			setter.Set(outer, 0, "Hello");

			Assert.IsNotNull(outer.Middle);
			Assert.AreEqual("Hello", outer.Middle.Value);
		}

		[Test]
		public void Should_set_second_level_value_with_one_array_indexer()
		{
			Expression<Func<Outer, string>> expression = o => o.Middle.Inners[0].Value;

			SafeProperty setter = SafeProperty.Create(expression);

			var outer = new Outer();
			setter.Set(outer, 0, "Hello");

			Assert.IsNotNull(outer.Middle);
			Assert.IsNotNull(outer.Middle.Inners);
			Assert.AreEqual(1, outer.Middle.Inners.Count);
			Assert.AreEqual("Hello", outer.Middle.Inners[0].Value);
		}

		[Test]
		public void Should_set_third_level_value_with_one_array_indexer()
		{
			Expression<Func<Outer, string>> expression = o => o.Middles[0].Inner.Value;

			SafeProperty setter = SafeProperty.Create(expression);

			var outer = new Outer();
			setter.Set(outer, 0, "Hello");

			Assert.IsNotNull(outer.Middles);
			Assert.AreEqual(1, outer.Middles.Count);
			Assert.IsNotNull(outer.Middles[0].Inner);
			Assert.AreEqual("Hello", outer.Middles[0].Inner.Value);
		}

		[Test]
		public void Should_set_third_level_value_with_two_array_indexers()
		{
			Expression<Func<Outer, int, string>> expression = (o, i) => o.Middles[0].Inners[i].Value;

			SafeProperty setter = SafeProperty.Create(expression);

			var outer = new Outer();
			setter.Set(outer, 0, "Hello");

			Assert.IsNotNull(outer.Middles);
			Assert.AreEqual(1, outer.Middles.Count);
			Assert.IsNotNull(outer.Middles[0].Inners);
			Assert.AreEqual(1, outer.Middles[0].Inners.Count);
			Assert.AreEqual("Hello", outer.Middles[0].Inners[0].Value);
		}

		public class Outer
		{
			public Middle Middle { get; set; }
			public IList<Middle> Middles { get; set; }
			public string Value { get; set; }
		}

		public class Middle
		{
			public Inner Inner { get; set; }
			public IList<Inner> Inners { get; set; }
			public string Value { get; set; }
		}

		public class Inner
		{
			public string Value { get; set; }
			public IList<string> Values { get; set; }
		}
	}
}