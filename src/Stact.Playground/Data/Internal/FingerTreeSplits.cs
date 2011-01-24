using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace FingerTree
{
	using Stact.Data.Internal;

    public delegate bool MPredicate<V> (V v); 

    public abstract partial class FTreeM<T, M> where T : Measurable<M>
    {
		public abstract Stact.Data.Split<T, FTreeM<T, M>, M> Split(MPredicate<M> predicate, M acc);

        public abstract Pair<FTreeM<T, M>, FTreeM<T, M>> SeqSplit(MPredicate<M> predicate);

        public FTreeM<T, M> takeUntil(MPredicate<M> predicate)
        {
            return SeqSplit(predicate).First;
        }

        public FTreeM<T, M> dropUntil(MPredicate<M> predicate)
        {
            return SeqSplit(predicate).Second;
        }

        public T Lookup(MPredicate<M> predicate, M acc)
        {
            return dropUntil(predicate).LeftView().head;
        }

        public partial class Digit<U, V> : Splittable<U, V>
              where U : Measurable<V>
        {
            // Assumption: predicate is false on the left end
            //             and true on the right end of the container
			public Stact.Data.Split<U, Digit<U, V>, V> Split(MPredicate<V> predicate, V acc)
            {
                int cnt = digNodes.Count;

                if (cnt == 0)
                    throw new Exception("Error: Split of an empty Digit attempted!");
                //else
                U headItem = digNodes[0];
                if(cnt == 1)
					return new Stact.Data.Split<U, Digit<U, V>, V>
                                  (new Digit<U, V>(theMonoid, new List<U>()),
                                   headItem,
                                   new Digit<U, V>(theMonoid, new List<U>())
                                  );
                //else
                List<U> digNodesTail = new List<U>(digNodes.GetRange(1, cnt - 1));
                Digit<U, V> digitTail = new Digit<U, V>(theMonoid, digNodesTail);

                V acc1 = theMonoid.Operation(acc, headItem.Measure());
                if (predicate(acc1))
					return new Stact.Data.Split<U, Digit<U, V>, V>
                                  (new Digit<U, V>(theMonoid, new List<U>()),
                                   headItem,
                                   digitTail
                                  );
                //else
				Stact.Data.Split<U, Digit<U, V>, V> tailSplit = digitTail.Split(predicate, acc1);

                tailSplit.Left.digNodes.Insert(0, headItem);

                return tailSplit;
            }
        }
    }

    public partial class EmptyFTreeM<T, M> : FTreeM<T, M> where T : Measurable<M>
    {
		public override Stact.Data.Split<T, FTreeM<T, M>, M> Split(MPredicate<M> predicate, M acc)
        {
            throw new Exception("Error: Split attempted on an EmptyFTreeM !");
        }

        public override Pair<FTreeM<T, M>, FTreeM<T, M>> SeqSplit(MPredicate<M> predicate)
        {
            return new Pair<FTreeM<T,M>,FTreeM<T,M>>
                     (new EmptyFTreeM<T, M>(theMonoid),
                      new EmptyFTreeM<T, M>(theMonoid)
                      );
        }
    }

    public partial class SingleFTreeM<T, M> : FTreeM<T, M> where T : Measurable<M>
    {
		public override Stact.Data.Split<T, FTreeM<T, M>, M> Split(MPredicate<M> predicate, M acc)
        {
			return new Stact.Data.Split<T, FTreeM<T, M>, M>
                         (new EmptyFTreeM<T, M>(theMonoid),
                          theSingle,
                          new EmptyFTreeM<T, M>(theMonoid)
                          );
        }

        public override Pair<FTreeM<T, M>, FTreeM<T, M>> SeqSplit(MPredicate<M> predicate)
        {
            M theMeasure = theSingle.Measure();

            if(predicate(theMeasure))
                return new Pair<FTreeM<T,M>,FTreeM<T,M>>
                         (new EmptyFTreeM<T, M>(theMonoid),
                          this
                          );
            //else
            return new Pair<FTreeM<T, M>, FTreeM<T, M>>
                     (this,
                      new EmptyFTreeM<T, M>(theMonoid)
                      );


        }

    }

    public partial class DeepFTreeM<T, M> : FTreeM<T, M> where T : Measurable<M>
    {
		public override Stact.Data.Split<T, FTreeM<T, M>, M> Split(MPredicate<M> predicate, M acc)
        {
            M vPr = theMonoid.Operation(acc, frontDig.Measure());

            if(predicate(vPr))
            {
				Stact.Data.Split<T, Digit<T, M>, M> 
                    frontSplit = frontDig.Split(predicate, acc);

				return new Stact.Data.Split<T, FTreeM<T, M>, M>
                            (FTreeM<T, M>.FromSequence(frontSplit.Left.digNodes, theMonoid),
                             frontSplit.Item,
                             FTreeM<T, M>.Create(frontSplit.Right.digNodes, innerFT, backDig)
                            );
            }
            //else
            M vM = theMonoid.Operation(vPr, innerFT.Measure());

            if (predicate(vM))
            {
                var midSplit = innerFT.Split(predicate, vPr);

                var midLeft = midSplit.Left;
                var midItem = midSplit.Item;

                var splitMidLeft =
                    (new Digit<T, M>(theMonoid, midItem.theNodes)).Split
                                                                (predicate, 
                                                                 theMonoid.Operation(vPr, midLeft.Measure())
                                                                );

                T finalsplitItem = splitMidLeft.Item;

                FTreeM<T, M> finalLeftTree =
                    FTreeM<T, M>.CreateR(frontDig, midLeft, splitMidLeft.Left.digNodes);

                FTreeM<T, M> finalRightTree =
                    FTreeM<T, M>.Create(splitMidLeft.Right.digNodes, midSplit.Right, backDig);

				return new Stact.Data.Split<T, FTreeM<T, M>, M>
                             (finalLeftTree, finalsplitItem, finalRightTree);

            }
            //else
			Stact.Data.Split<T, Digit<T, M>, M>
                backSplit = backDig.Split(predicate, vM);

			return new Stact.Data.Split<T, FTreeM<T, M>, M>
                        (FTreeM<T, M>.CreateR(frontDig,  innerFT, backSplit.Left.digNodes),
                         backSplit.Item,
                         FTreeM<T, M>.FromSequence(backSplit.Right.digNodes, theMonoid)
                        );
        }

        public override Pair<FTreeM<T, M>, FTreeM<T, M>> SeqSplit(MPredicate<M> predicate)
        {
            if(!predicate(Measure()))
                return new Pair<FTreeM<T, M>, FTreeM<T, M>>
                         (this,
                          new EmptyFTreeM<T, M>(theMonoid)
                          );
            //else
			Stact.Data.Split<T, FTreeM<T, M>, M> theSplit = Split(predicate, theMonoid.Zero);

            return new Pair<FTreeM<T, M>, FTreeM<T, M>>
                    (theSplit.Left, theSplit.Right.PushFront(theSplit.Item)
                     );
        }

    }
}