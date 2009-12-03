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
namespace Magnum.Generator
{
	using System;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;

	public class ObjectGenerator<T, TArg0> :
		ObjectGeneratorBase
	{
		private static ObjectGenerator<T, TArg0> _current;

		private Func<TArg0, T> _new;

		private ObjectGenerator()
			: base(typeof(T))
		{
			InitializeNew();
		}

		public static ObjectGenerator<T, TArg0> Current
		{
			get
			{
				if (_current == null)
					_current = new ObjectGenerator<T, TArg0>();

				return _current;
			}
		}

		private void InitializeNew()
		{
			_new = arg0 =>
				{
					ConstructorInfo constructorInfo = Constructors
						.MatchingArguments(arg0)
						.SingleOrDefault();

					if (constructorInfo == null)
						throw new ObjectGeneratorException(typeof(T), "No usable constructor found", typeof(TArg0));

					ParameterExpression parameter = constructorInfo.GetParameters().First().ToParameterExpression();

					Func<TArg0, T> lambda = Expression.Lambda<Func<TArg0, T>>(Expression.New(constructorInfo, parameter), parameter).Compile();

					_new = lambda;

					return lambda(arg0);
				};
		}

		public static T Create(TArg0 arg0)
		{
			return Current._new(arg0);
		}
	}
}