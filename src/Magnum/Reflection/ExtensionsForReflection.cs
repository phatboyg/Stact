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
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using Collections;
	using ObjectExtensions;
	using InterfaceExtensions;

	public static class ExtensionsForReflection
	{
		public static IEnumerable<Type> GetDeclaredGenericArguments(this object obj)
		{
			if (obj == null)
				yield break;

			foreach (var type in obj.GetType().GetDeclaredGenericArguments())
			{
				yield return type;
			}
		}

		public static IEnumerable<Type> GetDeclaredGenericArguments(this Type type)
		{
			bool atLeastOne = false;
			var baseType = type;
			while (baseType != null)
			{
				if (baseType.IsGenericType)
				{
					foreach (var declaredType in baseType.GetGenericArguments())
					{
						yield return declaredType;

						atLeastOne = true;
					}
				}

				baseType = baseType.BaseType;
			}

			if (atLeastOne)
				yield break;

			foreach (var interfaceType in type.GetInterfaces())
			{
				if (!interfaceType.IsGenericType)
					continue;

				foreach (var declaredType in interfaceType.GetGenericArguments())
				{
					if (declaredType.IsGenericParameter)
						continue;

					yield return declaredType;
				}
			}
		}

		public static IEnumerable<MethodInfo> GetMethodCandidates(this Type type, string methodName)
		{
			const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;

			return type.GetMethods(bindingFlags).Where(method => method.Name == methodName);
		}

		public static MethodInfo FindBestMatch(this IEnumerable<MethodInfo> methods, object[] args)
		{
			var highestScore = -1;
			var matchingMethodCount = 0;
			MethodInfo selectedMethod = null;

			foreach (var method in methods)
			{
				var methodScore = RateMethodMatch(method.GetParameters(), args);
				if (methodScore > highestScore)
				{
					matchingMethodCount = 1;
					highestScore = methodScore;
					selectedMethod = method;
				}
				else if (methodScore == highestScore)
				{
					// count the number of matches, match count > 1 => ambiguous call
					matchingMethodCount++;
				}
			}

			if (matchingMethodCount > 1)
				throw new ArgumentException("Ambiguous method invocation");

			if (selectedMethod == null)
				return null;

			if (selectedMethod.IsGenericMethod)
			{
				return selectedMethod.ToSpecializedMethod(args);
			}

			return selectedMethod;
		}

		public static T InvokeOn<T>(this MethodInfo method, object instance, object[] args)
		{
			method.MustNotBeNull("method");

			return (T) method.Invoke(instance, args);
		}

		private static MethodInfo ToSpecializedMethod(this MethodInfo methodInfo, object[] args)
		{
			if (!methodInfo.IsGenericMethod)
				return methodInfo;

			Type[] arguments = methodInfo.GetGenericArguments();

			var generics = new Dictionary<Type, Type>();

			arguments.Each(argument =>
				{
					methodInfo.GetParameters()
						.Where(parameter => parameter.ParameterType == argument)
						.Each(parameter =>
							{
								Type parameterType = args[parameter.Position].GetType();

								generics[argument] = parameterType;

								argument.GetGenericParameterConstraints()
									.Where(x => x.IsGenericType)
									.Each(constraint =>
									{
										var declared = constraint.GetDeclaredGenericArguments().ToArray();
										var specified = parameterType
											.GetDeclaredTypesForGeneric(constraint.GetGenericTypeDefinition()).ToArray();

										if(declared.Length == specified.Length)
										{
											for (int i = 0; i < declared.Length; i++)
											{
												if(arguments.Contains(declared[i]))
													generics[declared[i]] = specified[i];
											}
										}
									});
							});
				});

			var methodTypes = arguments.Select(x => generics[x]).ToArray();

			return methodInfo.GetGenericMethodDefinition().MakeGenericMethod(methodTypes);
		}

		/// <returns>0 if the arguments don't match the parameters; a score &gt; 0 otherwise.</returns>
		private static int RateMethodMatch(ParameterInfo[] parameters, object[] args)
		{
			var argsLength = args != null ? args.Length : 0;
			if (parameters.Length == argsLength)
			{
				return argsLength == 0 ? 1 : RateParameterMatches(parameters, args);
			}
			return 0;
		}

		private static int RateParameterMatches(ParameterInfo[] parameters, object[] args)
		{
			var score = 0;
			for (var i = 0; i < args.Length; ++i)
			{
				var typeMatchScore = RateParameterMatch(parameters[i], args[i]);
				if (typeMatchScore == 0)
				{
					return 0;
				}
				score += typeMatchScore;
			}
			return score;
		}

		private static int RateParameterMatch(ParameterInfo parameter, object arg)
		{
			var parameterType = parameter.ParameterType;
			return arg == null ? RateNullArgument(parameterType) : RateNonNullArgument(arg, parameterType);
		}

		private static int RateNullArgument(Type parameterType)
		{
			return CanBeNull(parameterType) ? 1 : 0;
		}

		private static bool CanBeNull(Type type)
		{
			return !type.IsValueType || IsNullableType(type);
		}

		private static bool IsNullableType(Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>);
		}

		private static int RateNonNullArgument(object arg, Type parameterType)
		{
			var argType = arg.GetType();
			if (argType == parameterType)
			{
				// perfect match!
				return 2;
			}
			if (parameterType.IsAssignableFrom(argType))
			{
				// at least convertible to parameter type
				return 1;
			}
			return 0;
		}
	}
}