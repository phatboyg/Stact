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
namespace Magnum.Reflection
{
	using System;
	using System.Linq.Expressions;
	using System.Reflection;

	public class TypeInstanceFactory<T> :
		InstanceFactory
	{
		private const BindingFlags _bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		private readonly Func<T> _new0;
		private readonly ConstructorInfo[] _constructors;
//		private Func<object, T> _new1;
//		private Func<object, object, T> _new2;
//		private Func<object, object, object, T> _new3;
//		private Func<object, object, object, object, T> _new4;

		public TypeInstanceFactory()
		{
			_constructors = typeof (T).GetConstructors(_bindingFlags);

			foreach (var constructor in _constructors)
			{
				if (constructor.GetParameters().Length != 0) continue;

				_new0 = Expression.Lambda<Func<T>>(Expression.New(constructor)).Compile();
			}
		}

		public object New()
		{
			return _new0();
		}

		public object New(params object[] args)
		{
			ConstantExpression[] constantArguments = new ConstantExpression[args.Length];
			for (int i = 0; i < args.Length; i++)
			{
				constantArguments[i] = Expression.Constant(args[i], args[i].GetType());
			}

			int currentArgUsage = 0;

			Expression<Func<T>> expression = null;

			foreach (var constructor in _constructors)
			{
				ParameterInfo[] parameters = constructor.GetParameters();

				// we can't make it if we don't have enough arguments
				if (parameters.Length != args.Length) continue;

				// if we found a greedier constructor, try to use it otherwise skip it
				if (parameters.Length < currentArgUsage) continue;

				expression = Expression.Lambda<Func<T>>(Expression.New(constructor, constantArguments));
				currentArgUsage = parameters.Length;
			}

			if(expression != null)
			{
				return expression.Compile()();
			}

			throw new InvalidOperationException("Could not make your object");
		}
	}
}