namespace FingerTree
{
	using System.Collections.Generic;
	using Stact.Data.Internal;


	public abstract partial class FTreeM<T, M> :
		Splittable<T, M> 
		where T : Measurable<M>
	{
		public abstract Monoid<M> treeMonoid { get; }

		public abstract M Measure();

		public abstract FTreeM<T, M> PushFront(T t);
		public abstract FTreeM<T, M> PushBack(T t);

		public abstract IEnumerable<T> ToSequence();
		public abstract IEnumerable<T> ToSequenceR();

		public abstract ViewL<T, M> LeftView();
		public abstract ViewR<T, M> RightView();

		public abstract FTreeM<T, M> Merge(FTreeM<T, M> rightFT);

		public abstract FTreeM<T, M> App2(List<T> ts, FTreeM<T, M> rightFT);

		public static FTreeM<T, M> FromSequence(IEnumerable<T> sequence, 
		                                        Monoid<M> aMonoid)
		{
			IEnumerator<T> sequenceEnum = sequence.GetEnumerator();

			FTreeM<T, M> ftResult = new EmptyFTreeM<T, M>(aMonoid);

			while (sequenceEnum.MoveNext())
			{
				ftResult = ftResult.PushBack(sequenceEnum.Current);
			}

			return ftResult;
		}

		public static FTreeM<T, M> Create(List<T> frontList,  //may be empty!
		                                  FTreeM<Node<T, M>, M> innerFT,
		                                  Digit<T, M> backDig
			)
		{
			Monoid<M> theMonoid = backDig.theMonoid;
            
			if (frontList.Count > 0)
				return new DeepFTreeM<T, M>
					(theMonoid,
					 new Digit<T, M>(theMonoid,
					                 frontList),
					 innerFT,
					 backDig
					);
			//else

			if (innerFT is EmptyFTreeM<Node<T, M>, M>)
				return FromSequence(backDig.digNodes, theMonoid);

			//else we must create a new intermediate tree
			var innerLeft = innerFT.LeftView();

			List<T> newlstFront = innerLeft.head.theNodes;

			DeepFTreeM<T, M> theNewDeepTree =
				new DeepFTreeM<T, M>
					(theMonoid, 
					 new Digit<T, M>(theMonoid, 
					                 newlstFront),
					 innerLeft.ftTail,
					 backDig
					);

			return theNewDeepTree;
		}


		public virtual FTreeM<T, M> Reverse()
		{
			return this;
		}

		public static Digit<T, M> ReverseDigit(Digit<T, M> theDigit)
		{
			List<T> newDigitList = new List<T>(theDigit.digNodes);

			newDigitList.Reverse();

			return new Digit<T, M>(theDigit.theMonoid, newDigitList);
		}

		public static FTreeM<T, M> CreateR(Digit<T, M> frontDig,
		                                   FTreeM<Node<T, M>, M> innerFT,
		                                   List<T> backList  //may be empty!
			)
		{
			Monoid<M> theMonoid = frontDig.theMonoid;

			if (backList.Count > 0)
				return new DeepFTreeM<T, M>
					(theMonoid, 
					 frontDig,
					 innerFT,
					 new Digit<T, M>(theMonoid, backList)
					);
			//else

			if (innerFT is EmptyFTreeM<Node<T, M>, M>)
				return FromSequence(frontDig.digNodes, theMonoid);

			//else we must create a new intermediate tree
			var innerRight = innerFT.RightView();

			List<T> newlstBack = innerRight.last.theNodes;

			DeepFTreeM<T, M> theNewDeepTree =
				new DeepFTreeM<T, M>
					(theMonoid, 
					 frontDig,
					 innerRight.ftInit,
					 new Digit<T, M>(theMonoid, newlstBack)
					);

			return theNewDeepTree;
		}

		public static List<Node<T, M>>
			ListOfNodes(Monoid<M> aMonoid, List<T> tList)
		{
			List<Node<T, M>> resultNodeList = new List<Node<T, M>>();

			Node<T, M> nextNode = null;

			int tCount = tList.Count;

			if (tCount < 4)
			{
				nextNode = new Node<T, M>(aMonoid, tList);

				resultNodeList.Add(nextNode);

				return resultNodeList;
			}

			//else
			List<T> nextTList = new List<T>(tList.GetRange(0, 3));

			nextNode = new Node<T, M>(aMonoid,nextTList);
			resultNodeList.Add(nextNode);

			resultNodeList.AddRange
				(ListOfNodes(aMonoid, 
				             tList.GetRange(3, tCount - 3)
				 	)
				);

			return resultNodeList;
		}


		public partial class Digit<U, V> : Splittable<U, V> 
			where U : Measurable<V>
		{
			public Monoid<V> theMonoid;

			public List<U> digNodes = new List<U>(); // At most four elements in this list

			public Digit(Monoid<V> aMonoid, U u1)
			{
				theMonoid = aMonoid;

				digNodes.Add(u1);
			}

			public Digit(Monoid<V> aMonoid, U u1, U u2)
			{
				theMonoid = aMonoid;

				digNodes.Add(u1);
				digNodes.Add(u2);
			}
			public Digit(Monoid<V> aMonoid, U u1, U u2, U u3)
			{
				theMonoid = aMonoid;

				digNodes.Add(u1);
				digNodes.Add(u2);
				digNodes.Add(u3);
			}
			public Digit(Monoid<V> aMonoid, U u1, U u2, U u3, U u4)
			{
				theMonoid = aMonoid;

				digNodes.Add(u1);
				digNodes.Add(u2);
				digNodes.Add(u3);
				digNodes.Add(u4);
			}

			public Digit(Monoid<V> aMonoid, List<U> listU)
			{
				theMonoid = aMonoid;

				digNodes = listU;
			}

			public V Measure()
			{
				V result = theMonoid.Zero;

				foreach (U u in digNodes)
					result = theMonoid.Operation(result, u.Measure());

				return result;
			}

			public IEnumerable<U> ToSequence()
			{
				return digNodes;
			}
		}

		public class Node<U, V> : Measurable<V> where U : Measurable<V>
		{
			public Monoid<V> theMonoid;

			protected V PreCalcMeasure;
            
			public List<U> theNodes = new List<U>(); // 2 or 3 elements in this list

			public Node(Monoid<V> aMonoid, U u1, U u2)
			{
				theMonoid = aMonoid;

				theNodes.Add(u1);
				theNodes.Add(u2);

				PreCalcMeasure = theMonoid.Operation(u1.Measure(), u2.Measure());
			}

			public Node(Monoid<V> aMonoid, U u1, U u2, U u3)
			{
				theMonoid = aMonoid;

				theNodes.Add(u1);
				theNodes.Add(u2);
				theNodes.Add(u3);

				PreCalcMeasure = theMonoid.Zero;
				foreach(U u in theNodes)
					PreCalcMeasure = theMonoid.Operation(PreCalcMeasure, u.Measure());
			}

			public Node(Monoid<V> aMonoid, List<U> listU)
			{
				theMonoid = aMonoid;

				theNodes = listU;

				PreCalcMeasure = theMonoid.Zero;
				foreach (U u in theNodes)
					PreCalcMeasure = theMonoid.Operation(PreCalcMeasure, u.Measure());
			}

			public V Measure()
			{
				return PreCalcMeasure;
			}
		}


		//public class ViewL<X, Y> where X : IMeasured<Y>
		//{
		//    public X head;
		//    public FTreeM<X, Y> ftTail;

		//    public ViewL(X head, FTreeM<X,Y> ftTail)
		//    {
		//        this.head = head;
		//        this.ftTail = ftTail;
		//    }
		//}

		//public class ViewR<X, Y> where X : IMeasured<Y>
		//{
		//    public X last;
		//    public FTreeM<X, Y> ftInit;

		//    public ViewR(FTreeM<X, Y> ftInit, X last)
		//    {
		//        this.ftInit = ftInit;
		//        this.last = last;
		//    }
		//}

	}
}