namespace FingerTree
{
	using Stact.Data.Internal;


	public class ViewR<X, Y> where X : Measurable<Y>
	{
		public X last;
		public FTreeM<X, Y> ftInit;

		public ViewR(FTreeM<X, Y> ftInit, X last)
		{
			this.ftInit = ftInit;
			this.last = last;
		}
	}
}