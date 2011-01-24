namespace Stact.Specs.Functional
{
	using System;
	using NUnit.Framework;


	[TestFixture]
	public class Binder
	{
		[Test]
		public void FirstTestName()
		{
			Func<A, B> aToB = a => new B(a);
			Func<B, C> bToC = b => new C(b);

			Func<A, C> aToC = aToB.Bind(bToC);

			C c = aToC(new A());

		}

		class A { }
		class B {
			readonly A _a;

			public B(A a)
			{
				_a = a;
			}
		}
		class C {
			readonly B _b;

			public C(B b)
			{
				_b = b;
			}
		}
	}

	public static class ExtensionsForFunc
	{
		public static Func<TA, TC> Bind<TA,TB,TC>(this Func<TA,TB> ab, Func<TB, TC> bc)
		{
			return a => bc(ab(a));
		}
	}
}
