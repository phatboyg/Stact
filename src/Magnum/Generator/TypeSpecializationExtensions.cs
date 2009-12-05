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
	using System.Reflection;
	using Collections;
	using InterfaceExtensions;
	using Linq;

	public static class TypeSpecializationExtensions
	{
		public static Type ToSpecializedType<T>(this T method, IEnumerable<object> args)
			where T : MethodBase
		{
			Guard.Against.Null(method, "method");

			Type type = method.DeclaringType;
			if (!type.IsGenericType)
				throw new ArgumentException("The argument must be for a generic type", "method");

			var parameters = method.GetParameters()
				.Merge(args, (x, y) => new {Parameter = x, Argument = y});

			Type[] arguments = type.GetGenericArguments();

			Type[] genericArguments = arguments.Join(GetResult(arguments, method, args), t => t, k => k.First, (a, b) => b)
				.Distinct()
				.Select(x => x.Second)
				.ToArray();

			if (arguments.Length != genericArguments.Length)
				throw new ObjectGeneratorException(type, "Unable to resolve generic arguments");

			return type.MakeGenericType(genericArguments);
		}

		private static IEnumerable<Tuple<Type, Type>> GetResult<T>(IEnumerable<Type> argumentTypes, T method, IEnumerable<object> args)
			where T : MethodBase
		{
			var parameters = method.GetParameters()
				.Merge(args, (x, y) => new {Parameter = x, Argument = y});

			foreach (var argType in argumentTypes)
			{
				var argumentType = argType;

				var matches = parameters
					.Where(arg => arg.Parameter.ParameterType == argumentType && arg.Argument != null)
					.Select(arg => arg.Argument.GetType());

				foreach (Type match in matches)
				{
					Type type = match;

					yield return new Tuple<Type, Type>(argumentType, type);

					var more = argumentType.GetGenericParameterConstraints()
						.Where(x => x.IsGenericType)
						.Where(x => type.Implements(x.GetGenericTypeDefinition()))
						.SelectMany(x => x.GetGenericArguments()
						                 	.Merge(type.GetDeclaredTypesForGeneric(x.GetGenericTypeDefinition()), (c, a) => new {Argument = c, Type = a}));

					foreach (var next in more)
					{
						yield return new Tuple<Type, Type>(next.Argument, next.Type);
					}
				}
			}
		}
	}
}