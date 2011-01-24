namespace FingerTree
{
	using System.Collections.Generic;
	using Stact.Data.Internal;


	public partial class SingleFTreeM<T, M> : FTreeM<T, M> where T : Measurable<M>
	{
		public Monoid<M> theMonoid;

		protected T theSingle;

		public SingleFTreeM(Monoid<M> aMonoid, T t) 
		{
			theMonoid = aMonoid;

			theSingle = t;
		}

		public override Monoid<M> treeMonoid
		{
			get { return theMonoid; }
		}

		public override M Measure()
		{
			return theSingle.Measure();
		}

		public override FTreeM<T, M> PushFront(T t)
		{
			return new DeepFTreeM<T, M>(theMonoid, 
			                            new Digit<T, M>(theMonoid, t),
			                            new EmptyFTreeM<Node<T, M>, M>(theMonoid),
			                            new Digit<T, M>(theMonoid, theSingle)
				);
		}

		public override FTreeM<T, M> PushBack(T t)
		{
			return new DeepFTreeM<T, M>(theMonoid, 
			                            new Digit<T, M>(theMonoid, theSingle), 
			                            new EmptyFTreeM<Node<T, M>, M>(theMonoid),
			                            new Digit<T, M>(theMonoid, t));
		}

		public override IEnumerable<T> ToSequence()
		{
			List<T> newL = new List<T>();
			newL.Add(theSingle);
			return newL;
		}

		public override IEnumerable<T> ToSequenceR()
		{
			List<T> newR = new List<T>();
			newR.Add(theSingle);
			return newR;
		}

		public override ViewL<T, M> LeftView()
		{
			return new ViewL<T, M>(theSingle, 
			                       new EmptyFTreeM<T, M>(theMonoid)
				);
		}

		public override ViewR<T,M> RightView()
		{
			return new ViewR<T, M>(new EmptyFTreeM<T, M>(theMonoid), 
			                       theSingle
				);
		}

		public override FTreeM<T, M> App2(List<T> ts, FTreeM<T, M> rightFT)
		{
			FTreeM<T, M> resultFT = rightFT;

			for (int i = ts.Count - 1; i >= 0; i--)
			{
				resultFT = resultFT.PushFront(ts[i]);
			}

			return resultFT.PushFront(theSingle);
		}

		public override FTreeM<T, M> Merge(FTreeM<T, M> rightFT)
		{
			return rightFT.PushFront(theSingle);
		}

	}
}