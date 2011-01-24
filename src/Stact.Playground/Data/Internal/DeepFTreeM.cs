using System;
using System.Collections.Generic;
using System.Text;

namespace FingerTree
{
	using Stact.Data.Internal;

	// Types:
    //       T  -- the Node type for this tree, must be measurable
    //       M  -- the type for the values of the measures


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
    