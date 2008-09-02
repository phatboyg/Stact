namespace Magnum.Common
{
	using System;
	using System.Collections.Generic;

	public static class DictionaryExtensions
	{
		/// <summary>
		/// Gets the value for the specified key or adds a new value to the dictionary
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="dictionary"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public static TValue Retrieve<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
			where TValue : new()
		{
			TValue ret;
			if (!dictionary.TryGetValue(key, out ret))
			{
				ret = new TValue();
				dictionary[key] = ret;
			}
			return ret;
		}

		/// <summary>
		/// Gets the value for the specified key or adds a new value to the dictionary
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="dictionary"></param>
		/// <param name="key"></param>
		/// <param name="missingValue">The value to add if the key is not found</param>
		/// <returns></returns>
		public static TValue Retrieve<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue missingValue)
		{
			TValue ret;
			if (!dictionary.TryGetValue(key, out ret))
			{
				ret = missingValue;
				dictionary[key] = ret;
			}
			return ret;
		}

		/// <summary>
		/// Gets the value for the specified key or adds a new value to the dictionary
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="dictionary"></param>
		/// <param name="key"></param>
		/// <param name="valueProvider">The function to return the value to add if the key is not found</param>
		/// <returns></returns>
		public static TValue Retrieve<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueProvider)
		{
			TValue ret;
			if (!dictionary.TryGetValue(key, out ret))
			{
				ret = valueProvider();
				dictionary[key] = ret;
			}
			return ret;
		}
	}
}