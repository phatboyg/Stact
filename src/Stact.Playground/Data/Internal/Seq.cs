namespace FingerTree
{
	using System;
	using System.Collections.Generic;
	using Stact.Data.Internal;


	public class Seq<T> : FTreeM<SizedElement<T>, uint>
	{
		public FTreeM<SizedElement<T>, uint> treeRep =
			new EmptyFTreeM<SizedElement<T>, uint>(Size.theMonoid);

		private static  bool theLessThanIMethod2(uint n, uint i)
		{
			return n < i;
		}

		public Seq(IEnumerable<T> aList)
		{
			foreach (T t in aList)
				treeRep = treeRep.PushBack(new SizedElement<T>(t));
		}

		public Seq(FTreeM<SizedElement<T>, uint> elemTree)
		{
			treeRep = elemTree;
		}

		public override Monoid<uint> treeMonoid
		{
			get
			{
				return treeRep.treeMonoid;
			}
		}

		public override uint Measure()
		{
			return treeRep.Measure();
		}

		public override FTreeM<SizedElement<T>, uint> PushFront(SizedElement<T> t)
		{
			return new Seq<T>(treeRep.PushFront(t));
		}

		public override FTreeM<SizedElement<T>, uint> PushBack(SizedElement<T> t)
		{
			return new Seq<T>(treeRep.PushBack(t));
		}

		public override IEnumerable<SizedElement<T>> ToSequence()
		{
			return treeRep.ToSequence();
		}

		public override IEnumerable<SizedElement<T>> ToSequenceR()
		{
			return treeRep.ToSequenceR();
		}

		public override ViewL<SizedElement<T>, uint> LeftView()
		{
			ViewL<SizedElement<T>, uint> internLView = treeRep.LeftView();

			internLView.ftTail = new Seq<T>(internLView.ftTail);

			return internLView;
		}

		public override ViewR<SizedElement<T>, uint> RightView()
		{
			ViewR<SizedElement<T>, uint> internRView = treeRep.RightView();
			internRView.ftInit = new Seq<T>(internRView.ftInit);

			return internRView;
		}

		public override FTreeM<SizedElement<T>, uint> Reverse()
		{
			return new Seq<T>(treeRep.Reverse());
		}

		public override FTreeM<SizedElement<T>, uint> Merge(FTreeM<SizedElement<T>, uint> rightFT)
		{
			//if (!(rightFT is Seq<T>))
			//    throw new Exception("Error: Seq merge with non-Seq attempted!");
			////else
			return treeRep.Merge(rightFT);
		}

		public override Stact.Data.Split<SizedElement<T>, FTreeM<SizedElement<T>, uint>, uint>
			Split(MPredicate<uint> predicate, uint acc)
		{
			Stact.Data.Split<SizedElement<T>, FTreeM<SizedElement<T>, uint>, uint> internSplit
				= treeRep.Split(predicate, acc);

			internSplit = new Stact.Data.Split<SizedElement<T>, FTreeM<SizedElement<T>, uint>, uint>(new Seq<T>(internSplit.Left), internSplit.Item, new Seq<T>(internSplit.Right));

			return internSplit;
		}

		public override Pair<FTreeM<SizedElement<T>, uint>, FTreeM<SizedElement<T>, uint>>
			SeqSplit(MPredicate<uint> predicate)
		{
			Pair<FTreeM<SizedElement<T>, uint>, FTreeM<SizedElement<T>, uint>> internPair
				= treeRep.SeqSplit(predicate);

			internPair = new Pair<FTreeM<SizedElement<T>, uint>, FTreeM<SizedElement<T>, uint>>(new Seq<T>(internPair.First),
			                                                                                    new Seq<T>(internPair.Second));

			return internPair;
		}

		public override FTreeM<SizedElement<T>, uint>
			App2(List<SizedElement<T>> ts, FTreeM<SizedElement<T>, uint> rightFT)
		{
			return treeRep.App2(ts, rightFT);
		}



		public uint length
		{
			get { return treeRep.Measure(); }
		}

		public Pair<Seq<T>, Seq<T>> SplitAt(uint ind)
		{
			var treeSplit = 
				treeRep.SeqSplit(new MPredicate<uint>
				                 	(FP.Curry<uint, uint, bool>(theLessThanIMethod2, ind))
					);
			return new Pair<Seq<T>, Seq<T>>
				(new Seq<T>(treeSplit.First),
				 new Seq<T>(treeSplit.Second)
				);
		}

		public T ElemAt(uint ind)
		{
			return treeRep.Split(new MPredicate<uint>
			                     	(FP.Curry<uint, uint, bool>(theLessThanIMethod2, ind)),
			                     0
				).Item.Value;
		}

		public T this[uint index]
		{
			get
			{
				return ElemAt(index);
			}
		}

		public Seq<T> InsertAt(uint index, T t)
		{
			if (index > length)
				throw new IndexOutOfRangeException
					(string.Format("Error: Attempt to insert at position: {0} "
					               + "exceeding the length: {1} of this sequence.",
					               index,
					               length
					 	)
					);
			//else
			Pair<Seq<T>, Seq<T>> theSplit = this.SplitAt(index);

			return new Seq<T>
				(
				theSplit.First.Merge(theSplit.Second.PushFront(new SizedElement<T>(t)))
				);
		}

		public Seq<T> RemoveAt(uint index)
		{
			if (index > length)
				throw new IndexOutOfRangeException
					(string.Format("Error: Attempt to remove at position: {0} "
					               + "exceeding the length: {1} of this sequence.",
					               index,
					               length
					 	)
					);
			//else
			Pair<Seq<T>, Seq<T>> theSplit = this.SplitAt(index);

			return new Seq<T>
				(
				theSplit.First.treeRep.Merge(theSplit.Second.treeRep.LeftView().ftTail)
				);
		}

	}
}