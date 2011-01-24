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