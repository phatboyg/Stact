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
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using CollectionExtensions;

	public class ObjectGenerator<T> :
		ObjectGeneratorBase,
		IObjectGenerator<T>
	{
		private static ObjectGenerator<T> _current;

		private Dictionary<int, Func<object[], T>> _argGenerators;
		private Func<T> _new;

		private ObjectGenerator()
			: base(typeof (T))
		{
			_argGenerators = new Dictionary<int, Func<object[], T>>();

			InitializeNew();
		}

		public static ObjectGenerator<T> Current
		{
			get
			{
				if (_current == null)
					_current = new ObjectGenerator<T>();

				return _current;
			}
		}

		object IObjectGenerator.Create()
		{
			return Create();
		}

		object IObjectGenerator.Create(object[] args)
		{
			return CreateFromArgs(args);
		}

		object IObjectGenerator.Create<TArg0>(TArg0 arg0)
		{
			return Create(arg0);
		}

		object IObjectGenerator.Create<TArg0, TArg1>(TArg0 arg0, TArg1 arg1)
		{
			return Create(arg0, arg1);
		}

		T IObjectGenerator<T>.Create()
		{
			return Create();
		}

		T IObjectGenerator<T>.Create(object[] args)
		{
			return CreateFromArgs(args);
		}

		T IObjectGenerator<T>.Create<TArg0>(TArg0 arg0)
		{
			return ObjectGenerator<T, TArg0>.Create(arg0);
		}

		T IObjectGenerator<T>.Create<TArg0, TArg1>(TArg0 arg0, TArg1 arg1)
		{
			return ObjectGenerator<T, TArg0, TArg1>.Create(arg0, arg1);
		}

		private void InitializeNew()
		{
			_new = () =>
				{
					ConstructorInfo constructorInfo = Constructors
						.MatchingArguments()
						.SingleOrDefault();

					if (constructorInfo == null)
						throw new ObjectGeneratorException(typeof (T), "No usable constructor found");

					Func<T> lambda = Expression.Lambda<Func<T>>(Expression.New(constructorInfo)).Compile();

					_new = lambda;

					return lambda();
				};
		}

		private T CreateFromArgs(object[] args)
		{
			int key = args.Aggregate(0, (x, o) => x ^ o.GetType().GetHashCode());

			var generator = _argGenerators.Retrieve(key, () =>
				{
					ConstructorInfo constructorInfo = Constructors
						.MatchingArguments(args)
						.SingleOrDefault();

					if (constructorInfo == null)
						throw new ObjectGeneratorException(typeof (T), "No usable constructor found");

					ParameterExpression argsParameter = Expression.Parameter(typeof (object[]), "args");

					Expression[] parameters = constructorInfo.GetParameters().ToObjectArrayExpression(argsParameter).ToArray();

					var newExpression = Expression.New(constructorInfo, parameters);

					Func<object[], T> lambda = Expression.Lambda<Func<object[], T>>(newExpression, argsParameter).Compile();

					return lambda;
				});

			return generator(args);
		}

		public static T Create()
		{
			return Current._new();
		}

		public static T Create<TArg0>(TArg0 arg0)
		{
			return ObjectGenerator<T, TArg0>.Create(arg0);
		}

		public static T Create<TArg0, TArg1>(TArg0 arg0, TArg1 arg1)
		{
			return ObjectGenerator<T, TArg0, TArg1>.Create(arg0, arg1);
		}
	}
}