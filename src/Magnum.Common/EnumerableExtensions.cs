namespace Magnum.Common
{
    using System.Collections.Generic;
    using Rhino.Commons.Exceptions;

    public static class EnumerableExtensions
    {
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
    }
}