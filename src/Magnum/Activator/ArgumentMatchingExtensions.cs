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
namespace Magnum.Activator
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using InterfaceExtensions;
	using Linq;

	public static class ArgumentMatchingExtensions
	{
		public static IEnumerable<T> MatchingArguments<T>(this IEnumerable<T> constructors)
			where T : MethodBase
		{
			return constructors
				.Where(x => x.GetParameters().MatchesArguments());
		}

		public static IEnumerable<T> MatchingArguments<T, TArg0>(this IEnumerable<T> methods, TArg0 arg0)
			where T : MethodBase
		{
			return methods
				.Where(x => x.GetParameters().MatchesArguments(arg0));
		}

		public static IEnumerable<T> MatchingArguments<T, TArg0, TArg1>(this IEnumerable<T> methods, TArg0 arg0, TArg1 arg1)
			where T : MethodBase
		{
			return methods
				.Where(x => x.GetParameters().MatchesArguments(arg0, arg1));
		}

		public static IEnumerable<T> MatchingArguments<T>(this IEnumerable<T> methods, object[] args)
			where T : MethodBase
		{
		    return methods
		        .Select(x => new {Method = x, Rating = x.GetParameters().MatchesArguments(args)})
		        .Where(x => x.Rating > 0)
		        .OrderByDescending(x => x.Rating)
		        .Select(x => x.Method);
		}

		public static bool MatchesArguments(this IEnumerable<ParameterInfo> parameters)
		{
			return parameters.Count() == 0;
		}

		public static bool MatchesArguments<TArg0>(this IEnumerable<ParameterInfo> parameters, TArg0 arg0)
		{
			ParameterInfo[] args = parameters.ToArray();

			return args.Length == 1 &&
			       args[0].ParameterType.RateParameterTypeCompatibility(typeof (TArg0)) > 0;
		}

		public static bool MatchesArguments<TArg0, TArg1>(this IEnumerable<ParameterInfo> parameters, TArg0 arg0, TArg1 arg1)
		{
			ParameterInfo[] args = parameters.ToArray();

			return args.Length == 2 &&
			       args[0].ParameterType.RateParameterTypeCompatibility(typeof (TArg0)) > 0 &&
			       args[1].ParameterType.RateParameterTypeCompatibility(typeof(TArg1)) > 0;
		}

		public static int MatchesArguments(this ParameterInfo[] parameterInfos, object[] args)
		{
			if (parameterInfos.Length != args.Length)
				return 0;

			if(parameterInfos.Length == 0)
				return 5;

            var matched = parameterInfos.Merge(args, (x, y) => new { Parameter = x, Argument = y, Rating = RateParameterTypeCompatibility(x.ParameterType, y) }).ToArray();

            int valid = matched
				.Where(x => x.Rating > 0)
				.Count();

            if (valid != args.Length)
                return 0;

		    return matched.Sum(x => x.Rating);
		}

		public static int RateParameterTypeCompatibility(this Type parameterType, object arg)
		{
			if (arg == null)
				return parameterType.CanBeNull() ? 1 : 0;

			return parameterType.RateParameterTypeCompatibility(arg.GetType());
		}

		private static bool CanBeNull(this Type type)
		{
			return !type.IsValueType || type.IsNullableType() || type == typeof(string);
		}

		private static bool IsNullableType(this Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		private static int RateParameterTypeCompatibility(this Type parameterType, Type argType)
		{
			if (argType == parameterType)
				return 4;

			if (parameterType.IsGenericParameter)
				return argType.MeetsGenericConstraints(parameterType) ? 3 : 0;

			if (parameterType.IsGenericType && 
				argType.IsGenericType && 
				parameterType.GetGenericTypeDefinition() == argType.GetGenericTypeDefinition())
				return 3;

			if (parameterType.IsAssignableFrom(argType))
			{
				// at least convertible to parameter type
				return 2;
			}

			return 0;
		}

		private static bool MeetsGenericConstraints(this Type type, Type genericType)
		{
			Type[] constraints = genericType.GetGenericParameterConstraints();

			int matched = constraints
				.Where(x => type.Implements(x.GetGenericTypeDefinition()))
				.Count();

			return matched == constraints.Length;
		}
	}
}