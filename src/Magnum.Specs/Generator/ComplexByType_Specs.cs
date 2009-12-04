namespace ComplexByType_Specs
{
	using System;
	using Magnum.Generator;
	using Magnum.Specs.Generator.Classes;
	using NUnit.Framework;

	[TestFixture]
	public class When_creating_an_object_with_multiple_arguments
	{
		private Type _objectType = typeof (ClassWithMultipleComplexConstructors);

		[Test]
		public void The_first_constructor_should_be_called()
		{
			object[] args = new object[] {47, "Name", 21, "Description"};

			var Instance = ObjectGenerator.Create(_objectType, args);

			Assert.IsNotNull(Instance);
			Assert.IsInstanceOfType(_objectType, Instance);
		}
	}
}
