// Copyright 2007-2008 The Apache Software Foundation.
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
namespace Magnum.ObjectExtensions
{
	using System;
	using System.Reflection;

    public static class ObjectExt
	{
		public static void MustNotBeNull<T>(this T reference) where T : class
		{
			if (reference == null)
				throw new ArgumentNullException();
		}

		public static void MustNotBeNull<T>(this T reference, string name) where T : class
		{
			if (reference == null)
				throw new ArgumentNullException(name);
		}

		public static void MustNotBeNull<T>(this T reference, string name, string message) where T : class
		{
			if (reference == null)
				throw new ArgumentNullException(name, message);
		}

		public static void MustNotBeEmpty(this string value)
		{
			if (String.IsNullOrEmpty(value))
				throw new ArgumentException();
		}

		public static void MustNotBeEmpty(this string value, string name)
		{
			if (String.IsNullOrEmpty(value))
				throw new ArgumentException("The argument must not be empty", name);
		}

		public static void MustNotBeEmpty(this string value, string name, string message)
		{
			if (String.IsNullOrEmpty(value))
				throw new ArgumentException(message, name);
		}

		public static void MustBeInRange<T>(this T value, RangeBuilder<T> rangeBuilder)
		{
			Range<T> range = rangeBuilder;

			value.MustBeInRange(range);
		}

		public static void MustBeInRange<T>(this T value, Range<T> range)
		{
			if (!range.Contains(value))
				throw new ArgumentException();
		}

        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// props to Jeremy Miller for these two nice helpers
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        /// <returns></returns>
	    public static T GetAttribute<T>(this ICustomAttributeProvider provider) where T : Attribute
	    {
	        var attributes = provider.GetCustomAttributes(typeof(T), true);
	        return attributes.Length > 0 ? attributes[0] as T : null;
	    }

        public static void ForAttributesOf<T>(this ICustomAttributeProvider provider, Action<T> action) where T : Attribute
        {
            foreach (T attribute in provider.GetCustomAttributes(typeof(T), true))
            {
                action(attribute);
            }
        }
	}
}