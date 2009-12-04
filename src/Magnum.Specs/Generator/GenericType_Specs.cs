namespace Magnum.Specs.Generator
{
	using Classes;
	using Magnum.Generator;
	using NUnit.Framework;

	[TestFixture]
	public class When_creating_a_generic_type
	{
		[Test]
		public void The_generic_arguments_should_be_used_to_build_the_type()
		{
			var obj = ObjectGenerator.Create(typeof (ClassWithOneGenericArgument<>), new[] {typeof (int)});

			obj.ShouldNotBeNull();
			obj.ShouldBeType<ClassWithOneGenericArgument<int>>();
		}

		[Test]
		public void The_generic_type_should_be_inferred_from_the_argument()
		{
			const int value = 27;

			var obj = ObjectGenerator.Create(typeof (ClassWithOneGenericArgument<>), value);

			obj.ShouldNotBeNull();
			obj.ShouldBeType<ClassWithOneGenericArgument<int>>();

			var instance = (ClassWithOneGenericArgument<int>) obj;

			instance.Value.ShouldEqual(value);
			instance.Count.ShouldEqual(default(int));
		}

		[Test]
		public void The_generic_type_should_be_inferred_from_the_first_argument()
		{
			const string value = "Name";
			const int count = 47;

			var obj = ObjectGenerator.Create(typeof (ClassWithOneGenericArgument<>), value, count);

			obj.ShouldNotBeNull();
			obj.ShouldBeType<ClassWithOneGenericArgument<string>>();

			var instance = (ClassWithOneGenericArgument<string>)obj;

			instance.Value.ShouldEqual(value);
			instance.Count.ShouldEqual(count);
		}
	}
}
