namespace Stact.Data.Internal
{
	using System.Collections.Generic;


	public abstract class FTree<T>
	{
		public abstract FTree<T> PushFront(T t);
		public abstract FTree<T> PushBack(T t);

		public abstract IEnumerable<T> ToSequence();
		public abstract IEnumerable<T> ToSequenceR();

		public abstract ViewL<T> LeftView();
		public abstract ViewR<T> RightView();

		public abstract FTree<T> Merge(FTree<T> rightFT);

		public abstract FTree<T> App2(List<T> ts, FTree<T> rightFT);


		public static FTree<T> Create(List<T> frontList, //may be empty!
		                              FTree<Node<T>> innerFT,
		                              Digit<T> backDig
			)
		{
			if (frontList.Count > 0)
			{
				return new DeepFTree<T>(new Digit<T>(frontList),
				                        innerFT,
				                        backDig
					);
			}
			//else

			if (innerFT is EmptyFTree<Node<T>>)
				return FromSequence(backDig.digNodes);

			//else we must create a new intermediate tree
			FTree<Node<T>>.ViewL<Node<T>> innerLeft = innerFT.LeftView();

			List<T> newlstFront = innerLeft.head.theNodes;

			var theNewDeepTree =
				new DeepFTree<T>(new Digit<T>(newlstFront),
				                 innerLeft.ftTail,
				                 backDig
					);

			return theNewDeepTree;
		}

		public static FTree<T> CreateR(Digit<T> frontDig,
		                               FTree<Node<T>> innerFT,
		                               List<T> backList //may be empty!
			)
		{
			if (backList.Count > 0)
			{
				return new DeepFTree<T>(frontDig,
				                        innerFT,
				                        new Digit<T>(backList)
					);
			}
			//else

			if (innerFT is EmptyFTree<Node<T>>)
				return FromSequence(frontDig.digNodes);

			//else we must create a new intermediate tree
			FTree<Node<T>>.ViewR<Node<T>> innerRight = innerFT.RightView();

			List<T> newlstBack = innerRight.last.theNodes;

			var theNewDeepTree =
				new DeepFTree<T>(frontDig,
				                 innerRight.ftInit,
				                 new Digit<T>(newlstBack)
					);

			return theNewDeepTree;
		}

		public static FTree<T> FromSequence(IEnumerable<T> sequence)
		{
			IEnumerator<T> sequenceEnum = sequence.GetEnumerator();

			FTree<T> ftResult = new EmptyFTree<T>();

			while (sequenceEnum.MoveNext())
				ftResult = ftResult.PushBack(sequenceEnum.Current);

			return ftResult;
		}

		public static List<Node<T>> ListOfNodes(List<T> tList)
		{
			var resultNodeList = new List<Node<T>>();

			Node<T> nextNode = null;

			int tCount = tList.Count;

			if (tCount < 4)
			{
				nextNode = new Node<T>(tList);

				resultNodeList.Add(nextNode);

				return resultNodeList;
			}

			//else
			var nextTList = new List<T>(tList.GetRange(0, 3));
			//tList.CopyTo(0, nextTList, 0, 3);

			nextNode = new Node<T>(nextTList);
			resultNodeList.Add(nextNode);

			resultNodeList.AddRange(ListOfNodes(tList.GetRange(3, tCount - 3)));

			return resultNodeList;
		}


		public class Digit<U>
		{
			public List<U> digNodes = new List<U>(); // At most four elements in this list

			public Digit(U u1)
			{
				digNodes.Add(u1);
			}

			public Digit(U u1, U u2)
			{
				digNodes.Add(u1);
				digNodes.Add(u2);
			}

			public Digit(U u1, U u2, U u3)
			{
				digNodes.Add(u1);
				digNodes.Add(u2);
				digNodes.Add(u3);
			}

			public Digit(U u1, U u2, U u3, U u4)
			{
				digNodes.Add(u1);
				digNodes.Add(u2);
				digNodes.Add(u3);
				digNodes.Add(u4);
			}

			public Digit(List<U> listU)
			{
				digNodes = listU;
			}
		}


		public class Node<V>
		{
			public List<V> theNodes = new List<V>(); // 2 or 3 elements in this list

			public Node(V v1, V v2)
			{
				theNodes.Add(v1);
				theNodes.Add(v2);
			}

			public Node(V v1, V v2, V v3)
			{
				theNodes.Add(v1);
				theNodes.Add(v2);
				theNodes.Add(v3);
			}

			public Node(List<V> listV)
			{
				theNodes = listV;
			}
		}


		public class ViewL<X>
		{
			public FTree<X> ftTail;
			public X head;

			public ViewL(X head, FTree<X> ftTail)
			{
				this.head = head;
				this.ftTail = ftTail;
			}
		}


		public class ViewR<X>
		{
			public FTree<X> ftInit;
			public X last;

			public ViewR(FTree<X> ftInit, X last)
			{
				this.ftInit = ftInit;
				this.last = last;
			}
		}
	}
}