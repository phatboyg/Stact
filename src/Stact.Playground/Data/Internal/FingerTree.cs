// Copyright 2010 Chris Patterson
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace Stact.Data.Internal
{
	using System;
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


	public class DeepFTree<T> : FTree<T>
	{
		protected Digit<T> backDig;
		protected Digit<T> frontDig;
		protected FTree<Node<T>> innerFT;

		public DeepFTree(Digit<T> frontDig, FTree<Node<T>> innerFT, Digit<T> backDig)
		{
			if (frontDig.digNodes.Count > 0)
			{
				this.frontDig = frontDig;
				this.innerFT = innerFT;
				this.backDig = backDig;
			}
			else
				throw new Exception("The DeepFTree() constructor is passed an empty frontDig !");
		}

		public override ViewL<T> LeftView()
		{
			T head = frontDig.digNodes[0];

			var newFront = new List<T>(frontDig.digNodes);
			newFront.RemoveAt(0);

			return new ViewL<T>(head,
			                    Create(newFront, innerFT, backDig)
				//new DeepFTree<T>(newDigs, innerFT, backDig)
				);
		}

		public override ViewR<T> RightView()
		{
			int lastIndex = backDig.digNodes.Count - 1;
			T last = backDig.digNodes[lastIndex];

			var newBack = new List<T>(backDig.digNodes);
			newBack.RemoveAt(lastIndex);

			return new ViewR<T>(CreateR(frontDig, innerFT, newBack),
			                    last
				);
		}

		public override FTree<T> PushFront(T t)
		{
			if (frontDig.digNodes.Count == 4)
			{
				var newFront = new List<T>(frontDig.digNodes);
				newFront.RemoveAt(0);

				return new DeepFTree<T>(new Digit<T>(t, frontDig.digNodes[0]),
				                        innerFT.PushFront(new Node<T>(newFront)),
				                        backDig
					);
			}
			else //less than  three digits in front -- will accomodate one more
			{
				var newFront = new List<T>(frontDig.digNodes);
				newFront.Insert(0, t);

				return new DeepFTree<T>(new Digit<T>(newFront), innerFT, backDig);
			}
		}

		public override FTree<T> PushBack(T t)
		{
			int cntbackDig = backDig.digNodes.Count;


			if (backDig.digNodes.Count == 4)
			{
				var newBack = new List<T>(backDig.digNodes);
				newBack.RemoveAt(cntbackDig - 1);

				return new DeepFTree<T>
					(frontDig,
					 innerFT.PushBack(new Node<T>(newBack)),
					 new Digit<T>(backDig.digNodes[cntbackDig - 1], t)
					);
			}
			else //less than  three digits at the back -- will accomodate one more
			{
				var newBack = new List<T>(backDig.digNodes);
				newBack.Add(t);

				return new DeepFTree<T>(frontDig, innerFT, new Digit<T>(newBack));
			}
		}


		public override IEnumerable<T> ToSequence()
		{
			ViewL<T> lView = LeftView();

			yield return lView.head;

			foreach (T t in lView.ftTail.ToSequence())
				yield return t;
		}

		public override IEnumerable<T> ToSequenceR()
		{
			ViewR<T> rView = RightView();

			yield return rView.last;

			foreach (T t in rView.ftInit.ToSequenceR())
				yield return t;
		}

		public override FTree<T> App2(List<T> ts, FTree<T> rightFT)
		{
			if (! (rightFT is DeepFTree<T>))
			{
				FTree<T> resultFT = this;

				foreach (T t in ts)
					resultFT = resultFT.PushBack(t);

				return (rightFT is EmptyFTree<T>)
				       	? resultFT
				       	: resultFT.PushBack(rightFT.LeftView().head);
			}
			else // the right tree is also a deep tree
			{
				var deepRight = rightFT as DeepFTree<T>;

				var cmbList = new List<T>(backDig.digNodes);

				cmbList.AddRange(ts);

				cmbList.AddRange(deepRight.frontDig.digNodes);

				FTree<T> resultFT =
					new DeepFTree<T>
						(frontDig,
						 innerFT.App2(ListOfNodes(cmbList), deepRight.innerFT),
						 deepRight.backDig
						);

				return resultFT;
			}
		}

		public override FTree<T> Merge(FTree<T> rightFT)
		{
			var emptyList = new List<T>();

			return App2(emptyList, rightFT);
		}
	}
}