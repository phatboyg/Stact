namespace Magnum.Common
{
    using System.Collections.Generic;
    using Rhino.Commons.Exceptions;

    public static class EnumerableExtensions
    {
        /// <summary>
        /// Get the first item in the collection, if it exists
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        public static T First<T>(this IEnumerable<T> collection)
        {
            var list = collection as IList<T>;
            if (list != null)
                return list[0];
            foreach (var item in collection)
            {
                return item;
            }
            throw new ElementNotFoundException();
        }

        /// <summary>
        /// Get the last element in the collection, if it exists
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        public static T Last<T>(this IEnumerable<T> collection)
        {
            var list = collection as IList<T>;
            if (list != null)
                return list[list.Count - 1];
            var last = default(T);
            var set = false;
            foreach (var item in collection)
            {
                last = item;
                set = true;
            }
            if (set)
                return last;
            throw new ElementNotFoundException();
        }
    }
}