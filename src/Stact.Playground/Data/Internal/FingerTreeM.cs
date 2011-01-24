using System;
using System.Collections.Generic;
using System.Text;

namespace FingerTree
{
	using Stact.Data.Internal;

	// Types:
    //       T  -- the Node type for this tree, must be measurable
    //       M  -- the type for the values of the measures
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

    public class ViewL<X, Y> where X : Measurable<Y>
    {
        public X head;
        public FTreeM<X, Y> ftTail;

        public ViewL(X head, FTreeM<X, Y> ftTail)
        {
            this.head = head;
            this.ftTail = ftTail;
        }
    }

    public class ViewR<X, Y> where X : Measurable<Y>
    {
        public X last;
        public FTreeM<X, Y> ftInit;

        public ViewR(FTreeM<X, Y> ftInit, X last)
        {
            this.ftInit = ftInit;
            this.last = last;
        }
    }


    public partial class EmptyFTreeM<T, M> : FTreeM<T, M> where T : Measurable<M>
    {
        Monoid<M> theMonoid;

        public EmptyFTreeM(Monoid<M> aMonoid) 
        {
            theMonoid = aMonoid;
        }

        public override Monoid<M> treeMonoid
        {
            get { return theMonoid; }
        }
        
        public override M Measure()
        {
            return theMonoid.Zero;
        }

        public override FTreeM<T, M> PushFront(T t)
        {
            return new SingleFTreeM<T, M>(theMonoid, t);
        }
        
        public override FTreeM<T, M> PushBack(T t)
        {
            return new SingleFTreeM<T, M>(theMonoid, t);
        }

        public override IEnumerable<T> ToSequence()
        {
            return new List<T>();
        }

        public override IEnumerable<T> ToSequenceR()
        {
            return new List<T>();
        }

        public override ViewL<T, M> LeftView()
        {
            return null;
        }

        public override ViewR<T, M> RightView()
        {
            return null;
        }

        public override FTreeM<T, M> App2(List<T> ts, FTreeM<T, M> rightFT)
        {
            FTreeM<T, M> resultFT = rightFT;

            for(int i = ts.Count -1; i >= 0; i--)
            {
                resultFT = resultFT.PushFront(ts[i]);
            }

            return resultFT;
        }

        public override FTreeM<T, M> Merge(FTreeM<T, M> rightFT)
        {
            return rightFT; 
        }

    }

    public partial class SingleFTreeM<T, M> : FTreeM<T, M> where T : Measurable<M>
    {
        public Monoid<M> theMonoid;

        protected T theSingle;

        public SingleFTreeM(Monoid<M> aMonoid, T t) 
        {
            theMonoid = aMonoid;

            theSingle = t;
        }

        public override Monoid<M> treeMonoid
        {
            get { return theMonoid; }
        }

        public override M Measure()
        {
            return theSingle.Measure();
        }

        public override FTreeM<T, M> PushFront(T t)
        {
            return new DeepFTreeM<T, M>(theMonoid, 
                                new Digit<T, M>(theMonoid, t),
                                new EmptyFTreeM<Node<T, M>, M>(theMonoid),
                                new Digit<T, M>(theMonoid, theSingle)
                                    );
        }

        public override FTreeM<T, M> PushBack(T t)
        {
            return new DeepFTreeM<T, M>(theMonoid, 
                                    new Digit<T, M>(theMonoid, theSingle), 
                                    new EmptyFTreeM<Node<T, M>, M>(theMonoid),
                                    new Digit<T, M>(theMonoid, t));
        }

        public override IEnumerable<T> ToSequence()
        {
            List<T> newL = new List<T>();
            newL.Add(theSingle);
            return newL;
        }

        public override IEnumerable<T> ToSequenceR()
        {
            List<T> newR = new List<T>();
            newR.Add(theSingle);
            return newR;
        }

        public override ViewL<T, M> LeftView()
        {
            return new ViewL<T, M>(theSingle, 
                                   new EmptyFTreeM<T, M>(theMonoid)
                                   );
        }

        public override ViewR<T,M> RightView()
        {
            return new ViewR<T, M>(new EmptyFTreeM<T, M>(theMonoid), 
                                   theSingle
                                   );
        }

        public override FTreeM<T, M> App2(List<T> ts, FTreeM<T, M> rightFT)
        {
            FTreeM<T, M> resultFT = rightFT;

            for (int i = ts.Count - 1; i >= 0; i--)
            {
                resultFT = resultFT.PushFront(ts[i]);
            }

            return resultFT.PushFront(theSingle);
        }

        public override FTreeM<T, M> Merge(FTreeM<T, M> rightFT)
        {
            return rightFT.PushFront(theSingle);
        }

    }

    public partial class DeepFTreeM<T, M> :
		FTreeM<T, M> 
		where T : Measurable<M>
    {
        public Monoid<M> theMonoid;

        protected M PreCalcMeasure;

        protected Digit<T, M> frontDig;
        protected FTreeM<Node<T, M>, M> innerFT;
        protected Digit<T, M> backDig;

        public DeepFTreeM(Monoid<M> aMonoid,

                          Digit<T, M> frontDig,
                          FTreeM<Node<T, M>, M> innerFT,
                          Digit<T, M> backDig)
        {
            if (frontDig.digNodes.Count > 0)
            {
                theMonoid = aMonoid;

                this.frontDig = frontDig;
                this.innerFT = innerFT;
                this.backDig = backDig;

                PreCalcMeasure = theMonoid.Zero;
                PreCalcMeasure = theMonoid.Operation(PreCalcMeasure, frontDig.Measure());
                PreCalcMeasure = theMonoid.Operation(PreCalcMeasure, innerFT.Measure());
                PreCalcMeasure = theMonoid.Operation(PreCalcMeasure, backDig.Measure());
            }
            else
            {
                throw new Exception("The DeepFTree() constructor is passed an empty frontDig !");
            }
        }

        public override Monoid<M> treeMonoid
        {
            get { return theMonoid; }
        }

        public override M Measure()
        {
            return PreCalcMeasure;
        }

        public override FTreeM<T, M> Reverse()
        {
            Digit<T, M> newFrontDig = ReverseDigit(backDig);
            Digit<T, M> newBackDig = ReverseDigit(frontDig);

            if (innerFT is EmptyFTreeM<Node<T, M>, M>)
                return new 
                 DeepFTreeM<T, M>(theMonoid, newFrontDig, innerFT, newBackDig);
            //else
            if (innerFT is SingleFTreeM<Node<T, M>, M>)
            {
                return new DeepFTreeM<T, M>
                  (theMonoid,
                   newFrontDig,
                   new SingleFTreeM<Node<T, M>, M>
                       (theMonoid,
                        ReverseNode(innerFT.LeftView().head)
                       ),
                   newBackDig
                   );
            }
            //else innerFT is a Deep tree
            DeepFTreeM<Node<T, M>, M> revDeepInner =
                (DeepFTreeM<Node<T, M>, M>)
                 (
                   ((DeepFTreeM<Node<T, M>, M>)innerFT).Reverse()
                  );

            List<Node<T, M>> newFrontNodes = new List<Node<T, M>>();
            List<Node<T, M>> newBackNodes = new List<Node<T, M>>();

            foreach (Node<T, M> node in revDeepInner.frontDig.digNodes)
                newFrontNodes.Add(ReverseNode(node));

            foreach (Node<T, M> node in revDeepInner.backDig.digNodes)
                newBackNodes.Add(ReverseNode(node));

            DeepFTreeM<Node<T, M>, M> reversedInner =
                new DeepFTreeM<Node<T, M>, M>
                  (theMonoid,
                   new DeepFTreeM<Node<T, M>, M>.Digit<Node<T, M>, M>
                                 (theMonoid, newFrontNodes),
                   revDeepInner.innerFT,
                   new DeepFTreeM<Node<T, M>, M>.Digit<Node<T, M>, M>
                                 (theMonoid, newBackNodes)
                   );

            return new DeepFTreeM<T, M>
              (theMonoid,
               ReverseDigit(backDig),
               reversedInner,
               ReverseDigit(frontDig)
               );
        }

        private static Node<T, M> ReverseNode(Node<T, M> aNode)
        {
            List<T> theNodes = new List<T>(aNode.theNodes);
            theNodes.Reverse();

            return new Node<T, M>(aNode.theMonoid, theNodes);
        }

        public override ViewL<T, M> LeftView()
        {
            T head = frontDig.digNodes[0];

            List<T> newFront = new List<T>(frontDig.digNodes);
            newFront.RemoveAt(0);

            return new ViewL<T, M>(head,
                                FTreeM<T, M>.Create(newFront, innerFT, backDig)
                                );
        }

        public override ViewR<T, M> RightView()
        {
            int lastIndex = backDig.digNodes.Count - 1;
            T last = backDig.digNodes[lastIndex];

            List<T> newBack = new List<T>(backDig.digNodes);
            newBack.RemoveAt(lastIndex);

            return new ViewR<T, M>(FTreeM<T,M>.CreateR(frontDig, innerFT, newBack),
                                last
                                );
        }

        public override FTreeM<T, M> PushFront(T t)
        {
            if (frontDig.digNodes.Count == 4)
            {
                List<T> newFront = new List<T>(frontDig.digNodes);
                newFront.RemoveAt(0);

                return new DeepFTreeM<T, M>
                     (theMonoid, 
                      new Digit<T, M>(theMonoid, 
                                      t, frontDig.digNodes[0]),
                      innerFT.PushFront(new Node<T, M>(theMonoid, newFront)),
                      backDig
                      );
            }
            else //less than  four digits in front -- will accomodate one more
            {
                List<T> newFront = new List<T>(frontDig.digNodes);
                newFront.Insert(0, t);

                return new DeepFTreeM<T, M>
                                  (theMonoid, 
                                   new Digit<T, M>(theMonoid,  
                                                   newFront), 
                                   innerFT, backDig);
            }
        }

        public override FTreeM<T, M> PushBack(T t)
        {
            int cntbackDig = backDig.digNodes.Count;


            if (backDig.digNodes.Count == 4)
            {
                List<T> newBack = new List<T>(backDig.digNodes);
                newBack.RemoveAt(cntbackDig - 1);

                return new DeepFTreeM<T, M>
                    (theMonoid, 
                     frontDig,
                     innerFT.PushBack(new Node<T, M>(theMonoid, newBack)),
                     new Digit<T, M>(theMonoid,  
                                     backDig.digNodes[cntbackDig - 1], 
                                     t)
                     );

            }
            else //less than  three digits at the back -- will accomodate one more
            {
                List<T> newBack = new List<T>(backDig.digNodes);
                newBack.Add(t);

                return new DeepFTreeM<T, M>
                      (theMonoid, 
                       frontDig, 
                       innerFT,
                       new Digit<T, M>(theMonoid, newBack));
            }

        }

        public override IEnumerable<T> ToSequence()
        {
            ViewL<T, M> lView = LeftView();

            yield return lView.head;

            foreach (T t in lView.ftTail.ToSequence())
                yield return t;
        }

        public override IEnumerable<T> ToSequenceR()
        {
            ViewR<T, M> rView = RightView();

            yield return rView.last;

            foreach (T t in rView.ftInit.ToSequenceR())
                yield return t;
        }


        public override FTreeM<T, M> App2(List<T> ts, FTreeM<T, M> rightFT)
        {
            if (rightFT is EmptyFTreeM<T, M>)
            {
                FTreeM<T, M> resultFT = this;

                foreach (T t in ts)
                {
                    resultFT = resultFT.PushBack(t);
                }

                return resultFT;

            }

            else if (rightFT is SingleFTreeM<T, M>)
            {
                FTreeM<T, M> resultFT = this;

                foreach (T t in ts)
                {
                    resultFT = resultFT.PushBack(t);
                }

                return resultFT.PushBack(rightFT.LeftView().head);

            }

            else // the right tree is also a deep tree
            {
                DeepFTreeM<T, M> deepRight = rightFT as DeepFTreeM<T, M>;

                List<T> cmbList = new List<T>(backDig.digNodes);

                cmbList.AddRange(ts);

                cmbList.AddRange(deepRight.frontDig.digNodes);

                FTreeM<T, M> resultFT =
                    new DeepFTreeM<T, M>
                            (theMonoid, 
                             frontDig,
                             innerFT.App2(FTreeM<T, M>.ListOfNodes
                                                     (theMonoid, cmbList), 
                                          deepRight.innerFT
                                          ),
                             deepRight.backDig
                             );

                return resultFT;
            }
        }


        public override FTreeM<T, M> Merge(FTreeM<T, M> rightFT)
        {
            List<T> emptyList = new List<T>();

            return App2(emptyList, rightFT);
        }

    }

}
    