namespace FingerTree
{
	using Stact.Data.Internal;


	public class CompElement<T> : Element<T, double>
	{
		private double dblRep;

		public CompElement(T t)
			: base(t)
		{
			dblRep = double.Parse(t.ToString());
		}

		public override double Measure()
		{
			return this.dblRep;
		}
	}
}