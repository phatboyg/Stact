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


	public class Deep<T, M> :
		FingerTree<T, M>
	{
		readonly FingerTree<Node<T, M>, M> _middle;
		readonly MakeTree<T, M> _mk;
		readonly Digit<T, M> _prefix;
		readonly Digit<T, M> _suffix;

		public Deep(Measured<T, M> measured, M size, Digit<T, M> prefix, FingerTree<Node<T, M>, M> middle,
		            Digit<T, M> suffix)
			: base(measured, size)
		{
			_mk = new MakeTree<T, M>(measured);

			_prefix = prefix;
			_middle = middle;
			_suffix = suffix;
		}


		public override LeftView<T, M> Left
		{
			get
			{
				T head = _prefix.Left;
				FingerTree<T, M> tail = _prefix.Match(x1 =>
					{
						if (_middle.IsEmpty)
							return _suffix.ToTree();

						var leftView = _middle.Left;

						return _mk.Deep(leftView.Head.Match<Digit<T, M>>(x2 => _mk.Two(x2.V1, x2.V2),
						                                                 x3 => _mk.Three(x3.V1, x3.V2, x3.V3)),
						                leftView.Tail, _suffix);
					},
				                                      x2 => _mk.Deep(_mk.One(x2.V2), _middle, _suffix),
				                                      x3 => _mk.Deep(_mk.Two(x3.V2, x3.V3), _middle, _suffix),
				                                      x4 => _mk.Deep(_mk.Three(x4.V2, x4.V3, x4.V4), _middle, _suffix));

				return new LeftView<T, M>(head, tail);
			}
		}

		public override RightView<T, M> Right
		{
			get
			{
				T head = _suffix.Right;
				FingerTree<T, M> tail = _suffix.Match(x1 =>
				{
					if (_middle.IsEmpty)
						return _prefix.ToTree();

					RightView<Node<T, M>, M> rightView = _middle.Right;

					return _mk.Deep(_prefix, rightView.Tail, 
						rightView.Head.Match<Digit<T, M>>(x2 => _mk.Two(x2.V1, x2.V2), x3 => _mk.Three(x3.V1, x3.V2, x3.V3)));
				},
													  x2 => _mk.Deep(_prefix, _middle, _mk.One(x2.V1)),
													  x3 => _mk.Deep(_prefix, _middle, _mk.Two(x3.V1, x3.V2)),
													  x4 => _mk.Deep(_prefix, _middle, _mk.Three(x4.V1, x4.V2, x4.V3)));

				return new RightView<T, M>(head, tail);
			}
		}

		public override U FoldRight<U>(Func<T, Func<U, U>> f, U z)
		{
			return _prefix.FoldRight(f, _middle.FoldRight(Flip(Node<T, M>.FoldRight(f)), _suffix.FoldRight(f, z)));
		}

		public override T ReduceRight(Func<T, Func<T, T>> f)
		{
			return _prefix.FoldRight(f, _middle.FoldRight(Flip(Node<T, M>.FoldRight(f)), _suffix.ReduceRight(f)));
		}

		static Func<B, Func<A, C>> Flip<A, B, C>(Func<A, Func<B, C>> f)
		{
			return b => a => f(a)(b);
		}

		public override U FoldLeft<U>(Func<U, Func<T, U>> f, U z)
		{
			return _suffix.FoldLeft(f, _middle.FoldLeft(Node<T, M>.FoldLeft(f), _prefix.FoldLeft(f, z)));
		}

		public override T ReduceLeft(Func<T, Func<T, T>> f)
		{
			return _suffix.FoldLeft(f, _middle.FoldLeft(Node<T, M>.FoldLeft(f), _prefix.ReduceLeft(f)));
		}


		public override FingerTree<U, M> Map<U>(Func<T, U> f, Measured<U, M> m)
		{
			return new Deep<U, M>(m, Size, _prefix.Map(f, m), _middle.Map(Node<T, M>.LiftM(f, m), m.Node), _suffix.Map(f, m));
		}

		public override U Match<U>(Func<Empty<T, M>, U> empty, Func<Single<T, M>, U> single, Func<Deep<T, M>, U> deep)
		{
			return deep(this);
		}

		public override bool Visit(Func<T, bool> callback)
		{
			return _prefix.Visit(callback) && _middle.Visit(x => x.Visit(callback)) && _suffix.Visit(callback);
		}

		public override FingerTree<T, M> AddLeft(T a)
		{
			Measured<T, M> m = Measured;
			M size = m.Append(m.Measure(a), Size);

			return _prefix.Match(x1 => new Deep<T, M>(m, size, _mk.Two(a, x1.V), _middle, _suffix),
			                     x2 => new Deep<T, M>(m, size, _mk.Three(a, x2.V1, x2.V2), _middle, _suffix),
			                     x3 => new Deep<T, M>(m, size, _mk.Four(a, x3.V1, x3.V2, x3.V3), _middle, _suffix),
			                     x4 => new Deep<T, M>(m, size, _mk.Two(a, x4.V1),
			                                          _middle.AddLeft(_mk.Node3(x4.V2, x4.V3, x4.V4)), _suffix));
		}

		public override FingerTree<T, M> AddRight(T a)
		{
			Measured<T, M> m = Measured;
			M measure = m.Append(m.Measure(a), Size);

			return _suffix.Match(x1 => new Deep<T, M>(m, measure, _prefix, _middle, _mk.Two(x1.V, a)),
			                     x2 => new Deep<T, M>(m, measure, _prefix, _middle, _mk.Three(x2.V1, x2.V2, a)),
			                     x3 => new Deep<T, M>(m, measure, _prefix, _middle, _mk.Four(x3.V1, x3.V2, x3.V3, a)),
			                     x4 => new Deep<T, M>(m, measure, _prefix, _middle.AddRight(_mk.Node3(x4.V1, x4.V2, x4.V3)),
			                                          _mk.Two(x4.V4, a)));
		}

		public override FingerTree<T, M> Concat(FingerTree<T, M> t)
		{
			Measured<T, M> m = Measured;
			return t.Match(e => this,
			               s => AddRight(s.Item),
			               d => new Deep<T, M>(m, m.Append(Size, d.Size), _prefix,
			                                   AddDigits0(m, _middle, _suffix, d._prefix, d._middle), d._suffix));
		}

		public override Pair<FingerTree<T, M>, FingerTree<T, M>> SplitSequence(MeasurePredicate<M> predicate)
		{
			if (!predicate(Size))
				return new Pair<FingerTree<T, M>, FingerTree<T, M>>(this, _mk.Empty());

			Split<T, FingerTree<T, M>, M> split = Split(predicate, Measured.Identity);

			return new Pair<FingerTree<T, M>, FingerTree<T, M>>(split.Left, split.Right.AddLeft(split.Item));
		}

		public override Split<T, FingerTree<T, M>, M> Split(MeasurePredicate<M> predicate, M acc)
		{
			var mk = new MakeTree<Node<T, M>, M>(Measured.Node);

			M prefixSize = Measured.Append(acc, _prefix.Size);
			if (predicate(prefixSize))
			{
				Split<T, Digit<T, M>, M> split = _prefix.Split(predicate, acc);

				FingerTree<T, M> leftTree = split.Left == null ? _mk.Empty() : split.Left.ToTree();
				FingerTree<T, M> rightTree;
				if (split.Right != null)
					rightTree = _mk.Deep(split.Right, _middle, _suffix);
				else
				{
					rightTree = _middle.Match(e => _suffix.ToTree(),
					                          s => _mk.Deep(s.Item.Match<Digit<T, M>>(x2 => _mk.Two(x2.V1, x2.V2),
					                                                                  x3 => _mk.Three(x3.V1, x3.V2, x3.V3)),
					                                        mk.Empty(), _suffix),
					                          d =>
					                          	{
					                          		var leftView = _middle.Left;
					                          		return _mk.Deep(leftView.Head.Match<Digit<T, M>>(x2 => _mk.Two(x2.V1, x2.V2),
					                          		                                                 x3 => _mk.Three(x3.V1, x3.V2, x3.V3)),
					                          		                leftView.Tail, _suffix);
					                          	});
				}

				return new Split<T, FingerTree<T, M>, M>(leftTree, split.Item, rightTree);
			}

			M middleSize = Measured.Append(prefixSize, _middle.Size);
			if (predicate(middleSize))
			{
				Split<Node<T, M>, FingerTree<Node<T, M>, M>, M> split = _middle.Split(predicate, prefixSize);

				Split<T, Digit<T, M>, M> splitMidLeft = split.Item
					.Match<Digit<T, M>>(x2 => _mk.Two(x2.V1, x2.V2), x3 => _mk.Three(x3.V1, x3.V2, x3.V3))
					.Split(predicate, Measured.Append(prefixSize, split.Left.Size));

				FingerTree<T, M> leftTree = _mk.Deep(_prefix, split.Left, splitMidLeft.Left);
				FingerTree<T, M> rightTree = _mk.Deep(splitMidLeft.Right, split.Right, _suffix);

				return new Split<T, FingerTree<T, M>, M>(leftTree, splitMidLeft.Item, rightTree);
			}

			Split<T, Digit<T, M>, M> splitSuffix = _suffix.Split(predicate, middleSize);
			FingerTree<T, M> right = splitSuffix.Right == null ? _mk.Empty() : splitSuffix.Right.ToTree();
			FingerTree<T, M> left;
			if (splitSuffix.Left != null)
				left = _mk.Deep(_prefix, _middle, splitSuffix.Left);
			else
			{
				left = _middle.Match(e => _prefix.ToTree(),
										  s => _mk.Deep(_prefix, mk.Empty(), s.Item.Match<Digit<T, M>>(x2 => _mk.Two(x2.V1, x2.V2),
																				  x3 => _mk.Three(x3.V1, x3.V2, x3.V3))),
										  d =>
										  {
											  var rightView = _middle.Right;
											  return _mk.Deep(_prefix, rightView.Tail, rightView.Head.Match<Digit<T, M>>(x2 => _mk.Two(x2.V1, x2.V2),
																							   x3 => _mk.Three(x3.V1, x3.V2, x3.V3)));
										  });
			}

			return new Split<T, FingerTree<T, M>, M>(left,splitSuffix.Item,right);
		}

		static FingerTree<Node<T, M>, M> AddDigits0<T, M>(Measured<T, M> m, FingerTree<Node<T, M>, M> m1,
		                                                  Digit<T, M> s1, Digit<T, M> p2, FingerTree<Node<T, M>, M> m2)
		{
			var mk = new MakeTree<T, M>(m);

			return s1.Match(x1 => p2.Match(y1 => Append1(m, m1, mk.Node2(x1.V, y1.V), m2),
			                               y2 => Append1(m, m1, mk.Node3(x1.V, y2.V1, y2.V2), m2),
			                               y3 => Append2(m, m1, mk.Node2(x1.V, y3.V1), mk.Node2(y3.V2, y3.V3), m2),
			                               y4 => Append2(m, m1, mk.Node3(x1.V, y4.V1, y4.V2), mk.Node2(y4.V3, y4.V4), m2)),
			                x2 => p2.Match(y1 => Append1(m, m1, mk.Node3(x2.V1, x2.V2, y1.V), m2),
			                               y2 => Append2(m, m1, mk.Node2(x2.V1, x2.V2), mk.Node2(y2.V1, y2.V2), m2),
			                               y3 => Append2(m, m1, mk.Node3(x2.V1, x2.V2, y3.V1), mk.Node2(y3.V2, y3.V3),
			                                             m2),
			                               y4 => Append2(m, m1, mk.Node3(x2.V1, x2.V2, y4.V1),
			                                             mk.Node3(y4.V2, y4.V3, y4.V4), m2)),
			                x3 => p2.Match(y1 => Append2(m, m1, mk.Node2(x3.V1, x3.V2), mk.Node2(x3.V3, y1.V), m2),
			                               y2 => Append2(m, m1, mk.Node3(x3.V1, x3.V2, x3.V3), mk.Node2(y2.V1, y2.V2), m2),
			                               y3 => Append2(m, m1, mk.Node3(x3.V1, x3.V2, x3.V3),
			                                             mk.Node3(y3.V1, y3.V2, y3.V3), m2),
			                               y4 => Append3(m, m1, mk.Node3(x3.V1, x3.V2, x3.V3), mk.Node2(y4.V1, y4.V2),
			                                             mk.Node2(y4.V3, y4.V4), m2)),
			                x4 => p2.Match(y1 => Append2(m, m1, mk.Node3(x4.V1, x4.V2, x4.V3), mk.Node2(x4.V4, y1.V), m2),
			                               y2 => Append2(m, m1, mk.Node3(x4.V1, x4.V2, x4.V3), mk.Node3(x4.V4, y2.V1, y2.V2), m2),
			                               y3 => Append3(m, m1, mk.Node3(x4.V1, x4.V2, x4.V3), mk.Node2(x4.V4, y3.V1),
			                                             mk.Node2(y3.V2, y3.V3), m2),
			                               y4 => Append3(m, m1, mk.Node3(x4.V1, x4.V2, x4.V3), mk.Node3(x4.V4, y4.V1, y4.V2),
			                                             mk.Node2(y4.V3, y4.V4), m2)));
		}


		static FingerTree<Node<T, M>, M> Append1<T, M>(Measured<T, M> m, FingerTree<Node<T, M>, M> t1, Node<T, M> n1,
		                                               FingerTree<Node<T, M>, M> t2)
		{
			return t1.Match(e => t2.AddLeft(n1),
			                s => t2.AddLeft(n1).AddLeft(s.Item),
			                d => t2.Match(e2 => t1.AddRight(n1),
			                              s2 => t1.AddRight(n1).AddRight(s2.Item),
			                              d2 =>
			                              new Deep<Node<T, M>, M>(m.Node, m.Append(m.Append(d.Size, m.Node.Measure(n1)), d2.Size),
			                                                      d._prefix,
			                                                      AddDigits1(m.Node, d._middle, d._suffix, n1, d2._prefix,
			                                                                 d2._middle),
			                                                      d2._suffix)));
		}

		static FingerTree<Node<T, M>, M> Append2<T, M>(Measured<T, M> m, FingerTree<Node<T, M>, M> t1, Node<T, M> n1,
		                                               Node<T, M> n2,
		                                               FingerTree<Node<T, M>, M> t2)
		{
			return t1.Match(e => t2.AddLeft(n2).AddLeft(n1),
			                s => t2.AddLeft(n2).AddLeft(n1).AddLeft(s.Item),
			                d => t2.Match(e2 => t2.AddRight(n1).AddRight(n2),
			                              s2 => t2.AddRight(n1).AddRight(n2).AddRight(s2.Item),
			                              d2 => new Deep<Node<T, M>, M>(m.Node,
			                                                            m.Append(
			                                                                     m.Append(m.Append(d.Size, n1.Measure),
			                                                                              n2.Measure),
			                                                                     d2.Size), d._prefix,
			                                                            AddDigits2(m.Node, d._middle, d._suffix, n1, n2,
			                                                                       d2._prefix,
			                                                                       d2._middle),
			                                                            d2._suffix)));
		}

		static FingerTree<Node<T, M>, M> Append3<T, M>(Measured<T, M> m, FingerTree<Node<T, M>, M> t1, Node<T, M> n1,
		                                               Node<T, M> n2,
		                                               Node<T, M> n3, FingerTree<Node<T, M>, M> t2)
		{
			return t1.Match(e => t2.AddLeft(n3).AddLeft(n2).AddLeft(n1),
			                s => t2.AddLeft(n3).AddLeft(n2).AddLeft(n1).AddLeft(s.Item),
			                d => t2.Match(e2 => t2.AddRight(n1).AddRight(n2).AddRight(n3),
			                              s2 => t2.AddRight(n1).AddRight(n2).AddRight(n3).AddRight(s2.Item),
			                              d2 => new Deep<Node<T, M>, M>(m.Node,
			                                                            m.Append(
			                                                                     m.Append(
			                                                                              m.Append(m.Append(d.Size, n1.Measure),
			                                                                                       n2.Measure),
			                                                                              n3.Measure),
			                                                                     d2.Size), d._prefix,
			                                                            AddDigits3(m.Node, d._middle, d._suffix, n1, n2, n3,
			                                                                       d2._prefix,
			                                                                       d2._middle),
			                                                            d2._suffix)));
		}

		static FingerTree<Node<T, M>, M> Append4<T, M>(Measured<T, M> m, FingerTree<Node<T, M>, M> t1, Node<T, M> n1,
		                                               Node<T, M> n2,
		                                               Node<T, M> n3, Node<T, M> n4, FingerTree<Node<T, M>, M> t2)
		{
			return t1.Match(e => t2.AddLeft(n4).AddLeft(n3).AddLeft(n2).AddLeft(n1),
			                s => t2.AddLeft(n4).AddLeft(n3).AddLeft(n2).AddLeft(n1).AddLeft(s.Item),
			                d => t2.Match(e2 => t2.AddRight(n1).AddRight(n2).AddRight(n3).AddRight(n4),
			                              s2 => t2.AddRight(n1).AddRight(n2).AddRight(n3).AddRight(n4).AddRight(s2.Item),
			                              d2 =>
			                              new Deep<Node<T, M>, M>(m.Node,
			                                                      m.Append(
			                                                               m.Append(
			                                                                        m.Append(
			                                                                                 m.Append(
			                                                                                          m.Append(d.Size, n1.Measure),
			                                                                                          n2.Measure), n3.Measure),
			                                                                        n4.Measure),
			                                                               d2.Size), d._prefix,
			                                                      AddDigits4(m.Node, d._middle, d._suffix, n1, n2, n3, n4,
			                                                                 d2._prefix,
			                                                                 d2._middle),
			                                                      d2._suffix)));
		}

		static FingerTree<Node<Node<T, M>, M>, M> AddDigits1<T, M>(Measured<Node<T, M>, M> m,
		                                                           FingerTree<Node<Node<T, M>, M>, M> m1,
		                                                           Digit<Node<T, M>, M> x, Node<T, M> n1,
		                                                           Digit<Node<T, M>, M> y,
		                                                           FingerTree<Node<Node<T, M>, M>, M> m2)
		{
			var mk = new MakeTree<Node<T, M>, M>(m);

			return x.Match(x1 => y.Match(y1 => Append1(m, m1, mk.Node3(x1.V, n1, y1.V), m2),
			                             y2 => Append2(m, m1, mk.Node2(x1.V, n1), mk.Node2(y2.V1, y2.V2), m2),
			                             y3 => Append2(m, m1, mk.Node3(x1.V, n1, y3.V1), mk.Node2(y3.V2, y3.V3), m2),
			                             y4 => Append2(m, m1, mk.Node3(x1.V, n1, y4.V1), mk.Node3(y4.V2, y4.V3, y4.V4), m2)),
			               x2 => y.Match(y1 => Append2(m, m1, mk.Node2(x2.V1, x2.V2), mk.Node2(n1, y1.V), m2),
			                             y2 => Append2(m, m1, mk.Node3(x2.V1, x2.V2, n1), mk.Node2(y2.V1, y2.V2), m2),
			                             y3 => Append2(m, m1, mk.Node3(x2.V1, x2.V2, n1), mk.Node3(y3.V1, y3.V2, y3.V3),
			                                           m2),
			                             y4 => Append3(m, m1, mk.Node3(x2.V1, x2.V2, n1), mk.Node2(y4.V1, y4.V2),
			                                           mk.Node2(y4.V3, y4.V4), m2)),
			               x3 => y.Match(y1 => Append2(m, m1, mk.Node3(x3.V1, x3.V2, x3.V3), mk.Node2(n1, y1.V), m2),
			                             y2 => Append2(m, m1, mk.Node3(x3.V1, x3.V2, x3.V3), mk.Node3(n1, y2.V1, y2.V2), m2),
			                             y3 => Append3(m, m1, mk.Node3(x3.V1, x3.V2, x3.V3), mk.Node2(n1, y3.V1),
			                                           mk.Node2(y3.V2, y3.V3), m2),
			                             y4 => Append3(m, m1, mk.Node3(x3.V1, x3.V2, x3.V3), mk.Node3(n1, y4.V1, y4.V2),
			                                           mk.Node2(y4.V3, y4.V4), m2)),
			               x4 => y.Match(y1 => Append2(m, m1, mk.Node3(x4.V1, x4.V2, x4.V3), mk.Node3(x4.V4, n1, y1.V), m2),
			                             y2 => Append3(m, m1, mk.Node3(x4.V1, x4.V2, x4.V3), mk.Node2(x4.V4, n1),
			                                           mk.Node2(y2.V1, y2.V2), m2),
			                             y3 => Append3(m, m1, mk.Node3(x4.V1, x4.V2, x4.V3), mk.Node3(x4.V4, n1, y3.V1),
			                                           mk.Node2(y3.V2, y3.V3), m2),
			                             y4 => Append3(m, m1, mk.Node3(x4.V1, x4.V2, x4.V3), mk.Node3(x4.V4, n1, y4.V1),
			                                           mk.Node3(y4.V2, y4.V3, y4.V4), m2)));
		}

		static FingerTree<Node<Node<T, M>, M>, M> AddDigits2<T, M>(Measured<Node<T, M>, M> m,
		                                                           FingerTree<Node<Node<T, M>, M>, M> m1,
		                                                           Digit<Node<T, M>, M> x, Node<T, M> n1, Node<T, M> n2,
		                                                           Digit<Node<T, M>, M> y,
		                                                           FingerTree<Node<Node<T, M>, M>, M> m2)
		{
			var mk = new MakeTree<Node<T, M>, M>(m);

			return x.Match(x1 => y.Match(y1 => Append2(m, m1, mk.Node2(x1.V, n1), mk.Node2(n2, y1.V), m2),
			                             y2 => Append2(m, m1, mk.Node3(x1.V, n1, n2), mk.Node2(y2.V1, y2.V2), m2),
			                             y3 => Append2(m, m1, mk.Node3(x1.V, n1, n2), mk.Node3(y3.V1, y3.V2, y3.V3), m2),
			                             y4 => Append3(m, m1, mk.Node3(x1.V, n1, n2), mk.Node2(y4.V1, y4.V2),
			                                           mk.Node2(y4.V3, y4.V4), m2)),
			               x2 => y.Match(y1 => Append2(m, m1, mk.Node3(x2.V1, x2.V2, n1), mk.Node2(n2, y1.V), m2),
			                             y2 => Append2(m, m1, mk.Node3(x2.V1, x2.V2, n1), mk.Node3(n2, y2.V1, y2.V2), m2),
			                             y3 => Append3(m, m1, mk.Node3(x2.V1, x2.V2, n1), mk.Node2(n2, y3.V1),
			                                           mk.Node2(y3.V2, y3.V3), m2),
			                             y4 => Append3(m, m1, mk.Node3(x2.V1, x2.V2, n1), mk.Node3(n2, y4.V1, y4.V2),
			                                           mk.Node2(y4.V3, y4.V4), m2)),
			               x3 => y.Match(y1 => Append2(m, m1, mk.Node3(x3.V1, x3.V2, x3.V3), mk.Node3(n1, n2, y1.V), m2),
			                             y2 => Append3(m, m1, mk.Node3(x3.V1, x3.V2, x3.V3), mk.Node2(n1, n2),
			                                           mk.Node2(y2.V1, y2.V2), m2),
			                             y3 => Append3(m, m1, mk.Node3(x3.V1, x3.V2, x3.V3), mk.Node3(n1, n2, y3.V1),
			                                           mk.Node2(y3.V2, y3.V3), m2),
			                             y4 => Append3(m, m1, mk.Node3(x3.V1, x3.V2, x3.V3), mk.Node3(n1, n2, y4.V1),
			                                           mk.Node3(y4.V2, y4.V3, y4.V4), m2)),
			               x4 => y.Match(y1 => Append3(m, m1, mk.Node3(x4.V1, x4.V2, x4.V3), mk.Node2(x4.V4, n1),
			                                           mk.Node2(n2, y1.V), m2),
			                             y2 => Append3(m, m1, mk.Node3(x4.V1, x4.V2, x4.V3), mk.Node3(x4.V4, n1, n2),
			                                           mk.Node2(y2.V1, y2.V2), m2),
			                             y3 => Append3(m, m1, mk.Node3(x4.V1, x4.V2, x4.V3), mk.Node3(x4.V4, n1, n2),
			                                           mk.Node3(y3.V1, y3.V2, y3.V3), m2),
			                             y4 => Append4(m, m1, mk.Node3(x4.V1, x4.V2, x4.V3), mk.Node3(x4.V4, n1, n2),
			                                           mk.Node2(y4.V1, y4.V2), mk.Node2(y4.V3, y4.V4), m2)));
		}

		static FingerTree<Node<Node<T, M>, M>, M> AddDigits3<T, M>(Measured<Node<T, M>, M> m,
		                                                           FingerTree<Node<Node<T, M>, M>, M> m1,
		                                                           Digit<Node<T, M>, M> x, Node<T, M> n1, Node<T, M> n2,
		                                                           Node<T, M> n3, Digit<Node<T, M>, M> y,
		                                                           FingerTree<Node<Node<T, M>, M>, M> m2)
		{
			var mk = new MakeTree<Node<T, M>, M>(m);

			return x.Match(x1 => y.Match(y1 => Append2(m, m1, mk.Node3(x1.V, n1, n2), mk.Node2(n3, y1.V), m2),
			                             y2 => Append2(m, m1, mk.Node3(x1.V, n1, n2), mk.Node3(n3, y2.V1, y2.V2), m2),
			                             y3 => Append3(m, m1, mk.Node3(x1.V, n1, n2), mk.Node2(n3, y3.V1),
			                                           mk.Node2(y3.V2, y3.V3), m2),
			                             y4 => Append3(m, m1, mk.Node3(x1.V, n1, n2), mk.Node3(n3, y4.V1, y4.V2),
			                                           mk.Node2(y4.V3, y4.V4), m2)),
			               x2 => y.Match(y1 => Append2(m, m1, mk.Node3(x2.V1, x2.V2, n1), mk.Node3(n2, n3, y1.V), m2),
			                             y2 => Append3(m, m1, mk.Node3(x2.V1, x2.V2, n1), mk.Node2(n2, n3),
			                                           mk.Node2(y2.V1, y2.V2), m2),
			                             y3 => Append3(m, m1, mk.Node3(x2.V1, x2.V2, n1), mk.Node3(n2, n3, y3.V1),
			                                           mk.Node2(y3.V2, y3.V3), m2),
			                             y4 => Append3(m, m1, mk.Node3(x2.V1, x2.V2, n1), mk.Node3(n2, n3, y4.V1),
			                                           mk.Node3(y4.V2, y4.V3, y4.V4), m2)),
			               x3 => y.Match(y1 => Append3(m, m1, mk.Node3(x3.V1, x3.V2, x3.V3), mk.Node2(n1, n2),
			                                           mk.Node2(n3, y1.V), m2),
			                             y2 => Append3(m, m1, mk.Node3(x3.V1, x3.V2, x3.V3), mk.Node3(n1, n2, n3),
			                                           mk.Node2(y2.V1, y2.V2), m2),
			                             y3 => Append3(m, m1, mk.Node3(x3.V1, x3.V2, x3.V3), mk.Node3(n1, n2, n3),
			                                           mk.Node3(y3.V1, y3.V2, y3.V3), m2),
			                             y4 => Append4(m, m1, mk.Node3(x3.V1, x3.V2, x3.V3), mk.Node3(n1, n2, n3),
			                                           mk.Node2(y4.V1, y4.V2), mk.Node2(y4.V3, y4.V4), m2)),
			               x4 => y.Match(y1 => Append3(m, m1, mk.Node3(x4.V1, x4.V2, x4.V3), mk.Node3(x4.V4, n1, n2),
			                                           mk.Node2(n3, y1.V), m2),
			                             y2 => Append3(m, m1, mk.Node3(x4.V1, x4.V2, x4.V3), mk.Node3(x4.V4, n1, n2),
			                                           mk.Node3(n3, y2.V1, y2.V2), m2),
			                             y3 => Append4(m, m1, mk.Node3(x4.V1, x4.V2, x4.V3), mk.Node3(x4.V4, n1, n2),
			                                           mk.Node2(n3, y3.V1), mk.Node2(y3.V2, y3.V3), m2),
			                             y4 => Append4(m, m1, mk.Node3(x4.V1, x4.V2, x4.V3), mk.Node3(x4.V4, n1, n2),
			                                           mk.Node3(n3, y4.V1, y4.V2), mk.Node2(y4.V3, y4.V4), m2)));
		}

		static FingerTree<Node<Node<T, M>, M>, M> AddDigits4<T, M>(Measured<Node<T, M>, M> m,
		                                                           FingerTree<Node<Node<T, M>, M>, M> m1,
		                                                           Digit<Node<T, M>, M> x, Node<T, M> n1, Node<T, M> n2,
		                                                           Node<T, M> n3, Node<T, M> n4, Digit<Node<T, M>, M> y,
		                                                           FingerTree<Node<Node<T, M>, M>, M> m2)
		{
			var mk = new MakeTree<Node<T, M>, M>(m);

			return x.Match(x1 => y.Match(y1 => Append2(m, m1, mk.Node3(x1.V, n1, n2), mk.Node3(n3, n4, y1.V), m2),
			                             y2 => Append3(m, m1, mk.Node3(x1.V, n1, n2), mk.Node2(n3, n4),
			                                           mk.Node2(y2.V1, y2.V2), m2),
			                             y3 => Append3(m, m1, mk.Node3(x1.V, n1, n2), mk.Node3(n3, n4, y3.V1),
			                                           mk.Node2(y3.V2, y3.V3), m2),
			                             y4 => Append3(m, m1, mk.Node3(x1.V, n1, n2), mk.Node3(n3, n4, y4.V1),
			                                           mk.Node3(y4.V2, y4.V3, y4.V4), m2)),
			               x2 => y.Match(y1 => Append3(m, m1, mk.Node3(x2.V1, x2.V2, n1), mk.Node2(n2, n3),
			                                           mk.Node2(n4, y1.V), m2),
			                             y2 => Append3(m, m1, mk.Node3(x2.V1, x2.V2, n1), mk.Node3(n2, n3, n4),
			                                           mk.Node2(y2.V1, y2.V2), m2),
			                             y3 => Append3(m, m1, mk.Node3(x2.V1, x2.V2, n1), mk.Node3(n2, n3, n4),
			                                           mk.Node3(y3.V1, y3.V2, y3.V3), m2),
			                             y4 => Append4(m, m1, mk.Node3(x2.V1, x2.V2, n1), mk.Node3(n2, n3, n4),
			                                           mk.Node2(y4.V1, y4.V2), mk.Node2(y4.V3, y4.V4), m2)),
			               x3 => y.Match(y1 => Append3(m, m1, mk.Node3(x3.V1, x3.V2, x3.V3), mk.Node3(n1, n2, n3),
			                                           mk.Node2(n4, y1.V), m2),
			                             y2 => Append3(m, m1, mk.Node3(x3.V1, x3.V2, x3.V3), mk.Node3(n1, n2, n3),
			                                           mk.Node3(n4, y2.V1, y2.V2), m2),
			                             y3 => Append4(m, m1, mk.Node3(x3.V1, x3.V2, x3.V3), mk.Node3(n1, n2, n3),
			                                           mk.Node2(n4, y3.V1), mk.Node2(y3.V2, y3.V3), m2),
			                             y4 => Append4(m, m1, mk.Node3(x3.V1, x3.V2, x3.V3), mk.Node3(n1, n2, n3),
			                                           mk.Node3(n4, y4.V1, y4.V2), mk.Node2(y4.V3, y4.V4), m2)),
			               x4 => y.Match(y1 => Append3(m, m1, mk.Node3(x4.V1, x4.V2, x4.V3), mk.Node3(x4.V4, n1, n2),
			                                           mk.Node3(n3, n4, y1.V), m2),
			                             y2 => Append4(m, m1, mk.Node3(x4.V1, x4.V2, x4.V3), mk.Node3(x4.V4, n1, n2),
			                                           mk.Node2(n3, n4), mk.Node2(y2.V1, y2.V2), m2),
			                             y3 => Append4(m, m1, mk.Node3(x4.V1, x4.V2, x4.V3), mk.Node3(x4.V4, n1, n2),
			                                           mk.Node3(n3, n4, y3.V1), mk.Node2(y3.V2, y3.V3), m2),
			                             y4 => Append4(m, m1, mk.Node3(x4.V1, x4.V2, x4.V3), mk.Node3(x4.V4, n1, n2),
			                                           mk.Node3(n3, n4, y4.V1), mk.Node3(y4.V2, y4.V3, y4.V4), m2)));
		}
	}
}