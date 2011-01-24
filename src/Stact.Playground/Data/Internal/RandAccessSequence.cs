using System;
using System.Text;

namespace FingerTree
{
	using Stact.Data.Internal;


	public static class Size
    {
        public static Monoid<uint> theMonoid =
             new Monoid<uint>(0, new Monoid<uint>.MonoidOperation(anAddOp));
        
        public static uint anAddOp(uint s1, uint s2)
        {
            return s1 + s2;
        }

    }

    public static class FP
    {
        public static Func<Y, Z> Curry<X, Y, Z>(this Func<X, Y, Z> func, X x)
        {
            return (y) => func(x, y);
        }

    }


	public class SizedElement<T> : Element<T, uint>
    {
        public SizedElement(T t) : base(t)
        {
        }

        public override uint Measure()
        {
            return 1;
        }
    }
}
