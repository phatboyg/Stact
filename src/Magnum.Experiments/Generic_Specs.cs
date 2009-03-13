namespace Magnum.Experiments
{
	using System;
	using NUnit.Framework;
	using Reflection;
	using TestFixtures;

	[TestFixture]
	public class Generic_Specs
	{
		[Test]
		public void I_want_some_nifty_helpers_to_make_generics_useful()
		{
			SimpleClass testClass = new SimpleClass();

			SingleGenericClass<Guid> single = new SingleGenericClass<Guid>(Guid.NewGuid());
			DoubleGenericClass<SingleGenericClass<Guid>, Guid> argument = new DoubleGenericClass<SingleGenericClass<Guid>, Guid>(single, single.Key);

			Generic.Call(testClass, x => x.DoubleGenericMethod<object, object>(null), argument);
		}
	}
}