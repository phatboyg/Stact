#region license

// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;
using Rhino.Commons.Exceptions;

namespace Rhino.Commons
{
    /// <summary>
    /// Helper class for collections
    /// </summary>
    public static class Collection
    {
        /// <summary>
        /// Return an IList containing all the items passed in the construtor.
        /// A cheap way to create a list from known variables, basically.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        public static IList<T> Containing<T>(params T[] items)
        {
            if (items == null) return null;
            return new List<T>(items);
        }

        /// <summary>
        /// Determines whether an element matching a predicate exists
        /// in the collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="predicate">The predicate to test.</param>
        /// <returns></returns>
        public static bool Exists<T>(IEnumerable<T> collection, Predicate<T> predicate)
        {
            foreach (T item in collection)
            {
                if (predicate(item))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Get the first item in the collection, if it exists
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        public static T First<T>(IEnumerable<T> collection)
        {
            IList<T> list = collection as IList<T>;
            if (list != null)
                return list[0];
            foreach (T item in collection)
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
        public static T Last<T>(IEnumerable<T> collection)
        {
            IList<T> list = collection as IList<T>;
            if (list != null)
                return list[list.Count - 1];
            T last = default(T);
            bool set = false;
            foreach (T item in collection)
            {
                last = item;
                set = true;
            }
            if (set)
                return last;
            throw new ElementNotFoundException();
        }

        /// <summary>
        /// Selects the items in the collection in the specified range.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection to get a range of elements from.</param>
        /// <param name="index">The index at which to start collecting elements.</param>
        /// <param name="count">The number of elements to collect.</param>
        /// <returns></returns>
        public static ICollection<T> Range<T>(IEnumerable<T> collection, int index, int count)
        {
            List<T> list = new List<T>(collection);

            Guard.Against<ArgumentOutOfRangeException>(list.Count == 0, "collection is empty");
            Guard.Against<ArgumentOutOfRangeException>(index < 0 || index > list.Count - 1, "index is out of range");
            Guard.Against<ArgumentOutOfRangeException>(count < 1 || count > list.Count - index, "count is out of range");

            return list.GetRange(index, count);
        }

        /// <summary>
        /// Selects all the items in the collection 
        /// that match the specified predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static ICollection<T> SelectAll<T>(IEnumerable<T> collection, Predicate<T> predicate)
        {
            return SelectInternal(true, collection, predicate);
        }

        /// <summary>
        /// Find the first item that match the given predicate, or
        /// default value if not element was matched
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        /// <param name="pred">The pred.</param>
        /// <returns></returns>
        public static T Find<T>(IEnumerable<T> items, Predicate<T> pred)
        {
            foreach (T item in items)
            {
                if (pred(item))
                    return item;
            }
            return default(T);
        }

        private static ICollection<T> SelectInternal<T>(bool addIfTrue, IEnumerable<T> collection, Predicate<T> predicate)
        {
            ICollection<T> results = new List<T>();
            foreach (T item in collection)
            {
                if (predicate(item))
                {
                    if (addIfTrue)
                        results.Add(item);
                }
                else if (addIfTrue == false)
                {
                    results.Add(item);
                }
            }
            return results;
        }

        /// <summary>
        /// Selects all items in the collection that does not 
        /// match the specified predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static ICollection<T> SelectAllNot<T>(IEnumerable<T> collection, Predicate<T> predicate)
        {
            return SelectInternal(false, collection, predicate);
        }

        /// <summary>
        /// Execute the action on all the items in the collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="action">The action.</param>
        public static void ForEach<T>(IEnumerable<T> collection, Action<T> action)
        {
            foreach (T item in collection)
            {
                action(item);
            }
        }

        /// <summary>
        /// Convert an untyped obj to array of specified type
        /// The type is assumed to implement IEnumerable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The list.</param>
        /// <returns></returns>
        public static T[] ToArray<T>(object obj)
        {
            IEnumerable enumerable = obj as IEnumerable;
            Guard.Against<ArgumentException>(enumerable == null, "obj did not implement IEnumerable");
            return ToArray<T>(enumerable);
        }

        /// <summary>
        /// Convert the given IEnumerable to an array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        public static T[] ToArray<T>(IEnumerable list)
        {
            List<T> items = new List<T>();
            foreach (T item in list)
            {
                items.Add(item);
            }
            return items.ToArray();
        }

        /// <summary>
        /// Converts the list to a binding list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        public static BindingList<T> ToBindingList<T>(IEnumerable list)
        {
            return new BindingList<T>(ToArray<T>(list));
        }

        /// <summary>
        /// Groups the items in the collection according to the grouping value
        /// returned from the converter
        /// </summary>
        /// <typeparam name="K">The item in the collection</typeparam>
        /// <typeparam name="T">The grouping item</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="converter">The converter.</param>
        /// <example>
        /// <code>
        ///	<![CDATA[
        /// int [] nums = { 1,2,3,4,5,6,7,8,9,10};
        /// IDictionary<bool, ICollection<int>> results = Collection.GroupBy<int, bool>(nums, delegate(int num) { return num % 2 == 0; });
        /// ]]>
        /// </code>
        /// Will output:
        /// { 
        /// 	true :  { 2,4,6,8,10 },
        /// 	false : { 1,3,5,7,9  },
        /// }
        /// </example>
        public static IDictionary<T, ICollection<K>> GroupBy<K, T>(ICollection<K> collection, Converter<K, T> converter)
        {
            Dictionary<T, ICollection<K>> dic = new Dictionary<T, ICollection<K>>();
            foreach (K k in collection)
            {
                T key = converter(k);
                if (dic.ContainsKey(key) == false)
                {
                    dic[key] = new List<K>();
                }
                dic[key].Add(k);
            }
            return dic;
        }

        /// <summary>
        /// Return a single element from the collection, and throw if 
        /// the collection has more than a single item
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        public static object Single(ICollection collection)
        {
            if (collection.Count == 0)
                return null;
            if (collection.Count > 1)
                throw new InvalidOperationException("Collection does not have exactly one item");
            IEnumerator enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            return enumerator.Current;
        }

        /// <summary>
        /// Return a collection where each item from the original collection 
        /// appear once and only once
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        public static ICollection<T> ToUniqueCollection<T>(IEnumerable<T> collection)
        {
            return ToUniqueCollection<T>((IEnumerable)collection);
        }

        /// <summary>
        /// Return a collection where each item from the original collection 
        /// appear once and only once
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        public static ICollection<T> ToUniqueCollection<T>(IEnumerable collection)
        {
            if (collection == null) return null;

            List<T> result = new List<T>();
            foreach (T item in collection)
            {
                if (!result.Contains(item))
                    result.Add(item);
            }
            return result;
        }

        /// <summary>
        /// Translate from one type to another using the converter
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="converter">The converter.</param>
        /// <returns></returns>
        public static IList<TResult> Map<TSource, TResult>(
            IEnumerable<TSource> source,
            Converter<TSource, TResult> converter)
        {
            List<TResult> result = new List<TResult>();
            if (source != null)
            {
                foreach (TSource item in source)
                    result.Add(converter(item));
            }
            return result;
        }

        /// <summary>
        /// Delegate for accumulating the values
        /// </summary>
        public delegate TResult Accumulator<TSource, TResult>(TSource source, TResult result);

        /// <summary>
        /// Reduces the specified source from a collection to the <typeparamref name="TResult"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="startValue">The start value.</param>
        /// <param name="accumulator">The accumulator.</param>
        /// <returns></returns>
        public static TResult Reduce<TSource, TResult>(
            IEnumerable<TSource> source, TResult startValue,
            Accumulator<TSource, TResult> accumulator)
        {
            TResult result = startValue;
            if (source != null)
            {
                foreach (TSource item in source)
                    result = accumulator(item, result);
            }
            return result;
        }
    }
}