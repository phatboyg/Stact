namespace Magnum.Specs.Generator
{
	using System;
	using Classes;
	using Magnum.Generator;
	using NUnit.Framework;

	[TestFixture]
	public class When_generating_an_object_with_arguments
	{
		private ClassWithOneConstructorArg _instance;

		[Test]
		public void The_object_should_be_created()
		{
			const int expected = 47;

			_instance = ObjectGenerator<ClassWithOneConstructorArg>.Create(expected);

			_instance.ShouldNotBeNull();
			_instance.ShouldBeType<ClassWithOneConstructorArg>();
			_instance.Value.ShouldEqual(expected);
			_instance.Name.ShouldBeNull();
		}

		[Test]
		public void The_object_should_be_created_with_the_name()
		{
			const string expected = "The Name";

			_instance = ObjectGenerator<ClassWithOneConstructorArg>.Create(expected);

			_instance.ShouldNotBeNull();
			_instance.ShouldBeType<ClassWithOneConstructorArg>();
			_instance.Value.ShouldEqual(default(int));
			_instance.Name.ShouldEqual(expected);
		}

		[Test]
		public void The_object_should_be_created_with_the_id()
		{
			Guid expected = CombGuid.Generate();

			_instance = ObjectGenerator<ClassWithOneConstructorArg>.Create(expected);

			_instance.ShouldNotBeNull();
			_instance.ShouldBeType<ClassWithOneConstructorArg>();
			_instance.Value.ShouldEqual(default(int));
			_instance.Name.ShouldBeNull();
			_instance.Id.ShouldEqual(expected);
		}
	}

	[TestFixture]
	public class When_generating_an_object_with_two_arguments
	{
		private ClassWithTwoConstructorArgs _instance;

		[Test]
		public void The_object_should_be_created()
		{
			const int expected = 47;
			const string expectedName = "The Name";

			_instance = ObjectGenerator<ClassWithTwoConstructorArgs>.Create(expected, expectedName);

			_instance.ShouldNotBeNull();
			_instance.ShouldBeType<ClassWithTwoConstructorArgs>();
			_instance.Value.ShouldEqual(expected);
			_instance.Name.ShouldEqual(expectedName);
		}
	}
}
