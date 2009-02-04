namespace Magnum.Specs.CommandLine
{
    using System;

    public static class ArrayExtensions
    {
        public static T[] Tail<T>(this T[] items) where T : class
        {
            int itemCount = items.Length == 0 ? 0 : items.Length - 1;
            T[] output = new T[itemCount];
            if(itemCount > 0)
                Array.Copy(items, 1, output, 0, itemCount);

            return output;
        }
        public static T Head<T>(this T[] items) where T : class
        {
            if (items.Length > 0)
                return items[0];

            return null;
        }
    }
}