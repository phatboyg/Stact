namespace FingerTree
{
	using Stact.Data.Internal;


	public static class Prio
	{
		public static Monoid<double> theMonoid =
			new Monoid<double>(double.NegativeInfinity, new Monoid<double>.MonoidOperation(aMaxOp));

		public static double aMaxOp(double d1, double d2)
		{
			return (d1 > d2) ? d1 : d2;
		}

	}
}