namespace FunctionalBits.Pipeline
{
    using System;
    using System.Collections.Generic;

    public static class CollectionExtensions
    {
        public static IEnumerable<T> Each<T>(this IEnumerable<T> collection, Action<T> callback)
        {
            foreach (T item in collection)
            {
                callback(item);
            }

            return collection;
        }

        public static IEnumerable<T> Each<T>(this IEnumerable<T> collection, Func<T, T> callback)
        {
            IList<T> result = new List<T>();

            foreach (T item in collection)
            {
                result.Add(callback(item));
            }

            return result;
        }
    }
}