namespace Magnum.Specs.Extensions
{
	using InterfaceExtensions;
	using MbUnit.Framework;

	[TestFixture]
	public class ExtensionsToInterfaces_Specs
	{
		public interface IGeneric<T> { }
		public class GenericClass : IGeneric<int> { }

		[Test]
		public void Open_generics_should_match_properly()
		{
			GenericClass genericClass = new GenericClass();

			Assert.IsTrue(genericClass.Implements(typeof(IGeneric<>)));
		}

	}
}