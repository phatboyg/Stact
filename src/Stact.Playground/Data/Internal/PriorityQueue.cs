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
namespace Stact.Data
{
	using System;
	using System.Collections.Generic;
	using FingerTree;
	using Internal;

	public class Split<T, U, M>
	{
		public readonly T Item;
		public readonly U Left;
		public readonly U Right;

		public Split(U left, T item, U right)
		{
			Left = left;
			Item = item;
			Right = right;
		}
	}

	public class PriorityQueue<T> :
		FTreeM<CompElement<T>, double>
	{
		readonly FTreeM<CompElement<T>, double> _treeRep =new EmptyFTreeM<CompElement<T>, double>(Prio.theMonoid);

		public PriorityQueue(IEnumerable<T> items)
		{
			foreach (T t in items)
				_treeRep = _treeRep.PushBack(new CompElement<T>(t));
		}

		public PriorityQueue(FTreeM<CompElement<T>, double> elemTree)
		{
			_treeRep = elemTree;
		}

		public override Stact.Data.Internal.Monoid<double> treeMonoid
		{
			get { return _treeRep.treeMonoid; }
		}

		static bool theLessOrEqMethod2(double d1, double d2)
		{
			return d1 <= d2;
		}

		public override double Measure()
		{
			return _treeRep.Measure();
		}

		public override FTreeM<CompElement<T>, double> PushFront(CompElement<T> elementT)
		{
			return new PriorityQueue<T>
				(_treeRep.PushFront(elementT));
		}


		public override FTreeM<CompElement<T>, double> PushBack(CompElement<T> elementT)
		{
			return new PriorityQueue<T>
				(_treeRep.PushBack(elementT));
		}

		public override IEnumerable<CompElement<T>> ToSequence()
		{
			return _treeRep.ToSequence();
		}

		public override IEnumerable<CompElement<T>> ToSequenceR()
		{
			return _treeRep.ToSequenceR();
		}


		public override ViewL<CompElement<T>, double> LeftView()
		{
			ViewL<CompElement<T>, double> internLView = _treeRep.LeftView();

			internLView.ftTail = new PriorityQueue<T>(internLView.ftTail);

			return internLView;
		}

		public override ViewR<CompElement<T>, double> RightView()
		{
			ViewR<CompElement<T>, double> internRView = _treeRep.RightView();
			internRView.ftInit = new PriorityQueue<T>(internRView.ftInit);

			return internRView;
		}

		public override FTreeM<CompElement<T>, double> Merge(FTreeM<CompElement<T>, double> rightFT)
		{
			if (!(rightFT is PriorityQueue<T>))
				throw new Exception("Error: PriQue merge with non-PriQue attempted!");
			//else
			return new PriorityQueue<T>
				(
				_treeRep.Merge(((PriorityQueue<T>)rightFT)._treeRep)
				);
		}

		public override Split<CompElement<T>, FTreeM<CompElement<T>, double>, double> Split(MPredicate<double> predicate, double acc)
		{
			Split<CompElement<T>, FTreeM<CompElement<T>, double>, double> internSplit
				= _treeRep.Split(predicate, acc);

			internSplit = new Split<CompElement<T>, FTreeM<CompElement<T>, double>, double>(new PriorityQueue<T>(internSplit.Left),
				internSplit.Item, new PriorityQueue<T>(internSplit.Right));

			return internSplit;
		}

		public override Pair<FTreeM<CompElement<T>, double>, FTreeM<CompElement<T>, double>>SeqSplit(MPredicate<double> predicate)
		{
			Pair<FTreeM<CompElement<T>, double>, FTreeM<CompElement<T>, double>> internPair
				= _treeRep.SeqSplit(predicate);
			internPair = new Pair<FTreeM<CompElement<T>, double>, FTreeM<CompElement<T>, double>>(new PriorityQueue<T>(internPair.First), new PriorityQueue<T>(internPair.Second));

			return internPair;
		}

		public override FTreeM<CompElement<T>, double>App2(List<CompElement<T>> ts, FTreeM<CompElement<T>, double> rightFT)
		{
			return _treeRep.App2(ts, rightFT);
		}

		public Pair<T, PriorityQueue<T>> extractMax()
		{
			Split<CompElement<T>, FTreeM<CompElement<T>, double>, double> trSplit =
				_treeRep.Split(new MPredicate<double>
				              	(FP.Curry<double, double, bool>
				              	 	(theLessOrEqMethod2, _treeRep.Measure())
				              	),
				              Prio.theMonoid.Zero
					);
			return new Pair<T, PriorityQueue<T>>
				(trSplit.Item.Value,
				 new PriorityQueue<T>(trSplit.Left.Merge(trSplit.Right))
				);
		}
	}
}