namespace Stact.Data.Internal
{
	using System.Collections.Generic;


	public class EmptyFTree<T> : FTree<T>
	{
		public override FTree<T> PushFront(T t)
		{
			return new SingleFTree<T>(t);
		}

		public override FTree<T> PushBack(T t)
		{
			return new SingleFTree<T>(t);
		}

		public override IEnumerable<T> ToSequence()
		{
			return new List<T>();
		}

		public override IEnumerable<T> ToSequenceR()
		{
			return new List<T>();
		}

		public override ViewL<T> LeftView()
		{
			return null;
		}

		public override ViewR<T> RightView()
		{
			return null;
		}

		public override FTree<T> App2(List<T> ts, FTree<T> rightFT)
		{
			FTree<T> resultFT = rightFT;

			for (int i = ts.Count - 1; i >= 0; i--)
				resultFT = resultFT.PushFront(ts[i]);

			return resultFT;
		}

		public override FTree<T> Merge(FTree<T> rightFT)
		{
			return rightFT;
		}
	}
}