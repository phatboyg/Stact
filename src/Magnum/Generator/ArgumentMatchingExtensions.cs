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
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	public static class ArgumentMatchingExtensions
	{
		public static IEnumerable<T> MatchingArguments<T>(this IEnumerable<T> constructors)
			where T : MethodBase
		{
			return constructors
				.Where(x => x.GetParameters().MatchesArguments());
		}

		public static IEnumerable<T> MatchingArguments<T, TArg0>(this IEnumerable<T> constructors, TArg0 arg0)
			where T : MethodBase
		{
			return constructors
				.Where(x => x.GetParameters().MatchesArguments(arg0));
		}

		public static IEnumerable<T> MatchingArguments<T, TArg0, TArg1>(this IEnumerable<T> constructors, TArg0 arg0, TArg1 arg1)
			where T : MethodBase
		{
			return constructors
				.Where(x => x.GetParameters().MatchesArguments(arg0, arg1));
		}

		public static bool MatchesArguments(this IEnumerable<ParameterInfo> parameters)
		{
			return parameters.Count() == 0;
		}

		public static bool MatchesArguments<TArg0>(this IEnumerable<ParameterInfo> parameters, TArg0 arg0)
		{
			ParameterInfo[] args = parameters.ToArray();

			return args.Length == 1 &&
			       args[0].ParameterType.IsAssignableFrom(typeof (TArg0));
		}

		public static bool MatchesArguments<TArg0, TArg1>(this IEnumerable<ParameterInfo> parameters, TArg0 arg0, TArg1 arg1)
		{
			ParameterInfo[] args = parameters.ToArray();

			return args.Length == 2 &&
			       args[0].ParameterType.IsAssignableFrom(typeof (TArg0)) &&
			       args[1].ParameterType.IsAssignableFrom(typeof (TArg1));
		}

//		public static ConstructorInfo FindBestMatch(this IEnumerable<ConstructorInfo> methods, object[] args)
//		{
//			var highestScore = -1;
//			var matchingMethodCount = 0;
//			ConstructorInfo selectedMethod = null;
//
//			foreach (var method in methods)
//			{
//				var methodScore = RateMethodMatch(method.GetParameters(), args);
//				if (methodScore > highestScore)
//				{
//					matchingMethodCount = 1;
//					highestScore = methodScore;
//					selectedMethod = method;
//				}
//				else if (methodScore == highestScore)
//				{
//					// count the number of matches, match count > 1 => ambiguous call
//					matchingMethodCount++;
//				}
//			}
//
//			if (matchingMethodCount > 1)
//				throw new ArgumentException("Ambiguous method invocation");
//
//			if (selectedMethod == null)
//				return null;
//
//			return selectedMethod;
//		}
	}
}