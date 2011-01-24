namespace Stact.Data.Internal
{
	using System.Collections.Generic;


	public class SingleFTree<T> : FTree<T>
	{
		protected T theSingle;

		public SingleFTree(T t)
		{
			theSingle = t;
		}

		public override FTree<T> PushFront(T t)
		{
			return new DeepFTree<T>(new Digit<T>(t),
			                        new EmptyFTree<Node<T>>(),
			                        new Digit<T>(theSingle)
				);
		}

		public override FTree<T> PushBack(T t)
		{
			return new DeepFTree<T>(new Digit<T>(theSingle),
			                        new EmptyFTree<Node<T>>(),
			                        new Digit<T>(t)
				);
		}

		public override IEnumerable<T> ToSequence()
		{
			var newL = new List<T>();
			newL.Add(theSingle);
			return newL;
		}

		public override IEnumerable<T> ToSequenceR()
		{
			var newR = new List<T>();
			newR.Add(theSingle);
			return newR;
		}

		public override ViewL<T> LeftView()
		{
			return new ViewL<T>(theSingle, new EmptyFTree<T>());
		}

		public override ViewR<T> RightView()
		{
			return new ViewR<T>(new EmptyFTree<T>(), theSingle);
		}

		public override FTree<T> App2(List<T> ts, FTree<T> rightFT)
		{
			FTree<T> resultFT = rightFT;

			for (int i = ts.Count - 1; i >= 0; i--)
				resultFT = resultFT.PushFront(ts[i]);

			return resultFT.PushFront(theSingle);
		}

		public override FTree<T> Merge(FTree<T> rightFT)
		{
			return rightFT.PushFront(theSingle);
		}
	}
}