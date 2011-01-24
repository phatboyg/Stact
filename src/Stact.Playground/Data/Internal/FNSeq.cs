namespace FingerTree
{
	using System;
	using System.Collections.Generic;
	using Stact.Data.Internal;


	public class FNSeq<T> //where T : IMeasured<uint>
	{
		private Seq<T> theSeq = null;

		public FNSeq()
		{
			theSeq = new Seq<T>(new List<T>());
		}

		public FNSeq(IEnumerable<T> seqIterator)
		{
			theSeq = new Seq<T>(new List<T>());

			foreach (T t in seqIterator)
				theSeq = (Seq<T>)(theSeq.PushBack(new SizedElement<T>(t)));
		}

		protected FNSeq(Seq<T> aSeq)
		{
			theSeq = aSeq;
		}

		public uint Length()
		{
			return this.theSeq.length;
		}

		public List<T> ToSequence()
		{
			List<T> lstResult = new List<T>();

			foreach (SizedElement<T> elem in theSeq.ToSequence())
				lstResult.Add(elem.Value);

			return lstResult;//.ToArray();
		}

		public T itemAt(int ind)
		{
			if (ind < 0 || ind >= Length())
				throw new ArgumentOutOfRangeException();
			//else
			return theSeq.ElemAt(((uint)ind));
		}

		public FNSeq<T> reverse()
		{
			return new FNSeq<T>((Seq<T>)(theSeq.Reverse()));
		}

		public FNSeq<T> Merge(FNSeq<T> seq2)
		{
			return new FNSeq<T>(new Seq<T>(theSeq.Merge(seq2.theSeq.treeRep)));
		}

		public FNSeq<T> skip(int length)
		{
			return new FNSeq<T>
				(
				new Seq<T>
					(
					this.theSeq.dropUntil(new MPredicate<uint>
					                      	(FP.Curry<uint, uint, bool>(theLTMethod, (uint)length))
						)
					)
				);
		}

		public FNSeq<T> take(int length)
		{
			return new FNSeq<T>
				(
				new Seq<T>
					(
					this.theSeq.takeUntil(new MPredicate<uint>
					                      	(FP.Curry<uint, uint, bool>(theLTMethod, (uint)length))
						)
					)
				);
		}

		public FNSeq<T> subsequence(int startInd, int subLength)
		{
			uint theLength = theSeq.length;

			if (theLength == 0 || subLength <= 0)
				return this;
			//else
			if (startInd < 0)
				startInd = 0;

			if (startInd + subLength > theLength)
				subLength = (int)(theLength - startInd);

			// Now ready to do the real work
			FNSeq<T> fsResult =
				new FNSeq<T>(
					(Seq<T>)
					(
						((Seq<T>)
						 (theSeq.SeqSplit
						 	(
						 	 new MPredicate<uint>
						 	 	(FP.Curry<uint, uint, bool>(theLTMethod, (uint)startInd))
						 	).Second
						 )
						).SeqSplit
						(new MPredicate<uint>
						 	(FP.Curry<uint, uint, bool>(theLTMethod, (uint)subLength))
						).First
					)
					);
			return fsResult;
		}

		public FNSeq<T> remove(int ind)
		{
			if (ind < 0 || ind >= Length())
				throw new ArgumentOutOfRangeException();
			//else
			return new FNSeq<T>(theSeq.RemoveAt((uint)(ind)));
		}

		// this inserts a whole sequence, so we cannot just use Seq.snsertAt()
		public FNSeq<T> insert_before(int ind, FNSeq<T> fSeq2)
		{
			if (ind < 0 || ind >= this.Length())
				throw new ArgumentOutOfRangeException();
			//else
			Pair<FTreeM<SizedElement<T>, uint>, FTreeM<SizedElement<T>, uint>> theSplit =
				theSeq.SeqSplit
					(new MPredicate<uint>
					 	(
					 	FP.Curry<uint, uint, bool>(theLTMethod, (uint)ind - 1)
					 	)
					);

			FNSeq<T> fs1 = new FNSeq<T>((Seq<T>)(theSplit.First));
			FNSeq<T> fs3 = new FNSeq<T>((Seq<T>)(theSplit.Second));

			return fs1.Merge(fSeq2).Merge(fs3);
		}

		bool theLTMethod(uint i1, uint i2)
		{
			return i1 < i2;
		}

	}
}