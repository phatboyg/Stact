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
namespace Magnum.Common.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public static class ReflectionCache<T>
    {
        private static readonly Dictionary<string, Func<T, object>> _getters = new Dictionary<string, Func<T, object>>();
        private static readonly Dictionary<string, Action<T, object>> _setters = new Dictionary<string, Action<T, object>>();

        public static object Get(string name, T instance)
        {
            if (_getters.ContainsKey(name))
                return _getters[name](instance);

            return GetFastProperty(name).GetDelegate(instance);
        }

        private static FastProperty<T> GetFastProperty(string name)
        {
            Type objectType = typeof (T);

            PropertyInfo pi = objectType.GetProperty(name);

            var fastProperty = new FastProperty<T>(pi);

            lock (_getters)
            {
                _getters[name] = fastProperty.GetDelegate;
                _setters[name] = fastProperty.SetDelegate;
            }

            return fastProperty;
        }

        public static void Set(string name, T instance, object value)
        {
            if (_getters.ContainsKey(name))
                _setters[name](instance, value);

            GetFastProperty(name).SetDelegate(instance, value);
        }

        public static IList<object> List(T instance)
        {
            List<object> values = new List<object>();

            PropertyInfo[] properties = typeof (T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo info in properties)
            {
                values.Add(Get(info.Name, instance));
            }

            return values;
        }

        public static IEnumerable<IReflectedObjectContext> GetEnumerator(T instance)
        {
            PropertyInfo[] properties = typeof (T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo info in properties)
            {
                yield return new ReflectedPropertyContext<T>(info, instance);
            }
        }
    }
}