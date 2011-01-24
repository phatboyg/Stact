namespace FingerTree
{
	using Stact.Data.Internal;


	public class ViewL<X, Y> where X : Measurable<Y>
	{
		public X head;
		public FTreeM<X, Y> ftTail;

		public ViewL(X head, FTreeM<X, Y> ftTail)
		{
			this.head = head;
			this.ftTail = ftTail;
		}
	}
}