namespace FingerTree
{
	using System.Collections.Generic;
	using Stact.Data.Internal;


	public partial class EmptyFTreeM<T, M> : FTreeM<T, M> where T : Measurable<M>
	{
		Monoid<M> theMonoid;

		public EmptyFTreeM(Monoid<M> aMonoid) 
		{
			theMonoid = aMonoid;
		}

		public override Monoid<M> treeMonoid
		{
			get { return theMonoid; }
		}
        
		public override M Measure()
		{
			return theMonoid.Zero;
		}

		public override FTreeM<T, M> PushFront(T t)
		{
			return new SingleFTreeM<T, M>(theMonoid, t);
		}
        
		public override FTreeM<T, M> PushBack(T t)
		{
			return new SingleFTreeM<T, M>(theMonoid, t);
		}

		public override IEnumerable<T> ToSequence()
		{
			return new List<T>();
		}

		public override IEnumerable<T> ToSequenceR()
		{
			return new List<T>();
		}

		public override ViewL<T, M> LeftView()
		{
			return null;
		}

		public override ViewR<T, M> RightView()
		{
			return null;
		}

		public override FTreeM<T, M> App2(List<T> ts, FTreeM<T, M> rightFT)
		{
			FTreeM<T, M> resultFT = rightFT;

			for(int i = ts.Count -1; i >= 0; i--)
			{
				resultFT = resultFT.PushFront(ts[i]);
			}

			return resultFT;
		}

		public override FTreeM<T, M> Merge(FTreeM<T, M> rightFT)
		{
			return rightFT; 
		}

	}
}