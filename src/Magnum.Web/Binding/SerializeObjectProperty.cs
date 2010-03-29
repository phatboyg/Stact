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
namespace Magnum.Web.Binding
{
	using System;
	using System.Linq.Expressions;
	using System.Reflection;

	public class SerializeObjectProperty<T>
	{
		private readonly Func<T, object> _get;
		private readonly PropertyInfo _info;

		public SerializeObjectProperty(PropertyInfo info)
		{
			_info = info;
			_get = InitializeGetMethod(info);
		}

		public string Name
		{
			get { return _info.Name; }
		}

		public Type PropertyType
		{
			get { return _info.PropertyType; }
		}

		public object GetValue(T instance)
		{
			return _get(instance);
		}

		private static Func<T, object> InitializeGetMethod(PropertyInfo info)
		{
			ParameterExpression instance = Expression.Parameter(typeof (T), "instance");

			MethodCallExpression call = Expression.Call(instance, info.GetGetMethod());
			UnaryExpression typeAs = Expression.TypeAs(call, typeof (object));
			return Expression.Lambda<Func<T, object>>(typeAs, instance).Compile();
		}
	}
}