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
namespace Magnum.InterfaceExtensions
{
	using System;
	using System.Collections.Generic;
	using Extensions;

	public static class ExtensionsToInterfaces
	{
//		public static Type GetDeclaredTypeForGeneric(this Type baseType, Type interfaceType)
//		{
//			var type = default(Type);
//
//			if (baseType.ImplementsGeneric(interfaceType))
//			{
//				var generic = baseType.GetInterface(interfaceType.FullName);
//				if (generic.IsGenericType)
//				{
//					var args = generic.GetGenericArguments();
//					if (args.Length == 1)
//					{
//						type = args[0];
//					}
//				}
//			}
//
//			return type;
//		}
//
//		public static IEnumerable<Type> GetDeclaredTypesForGeneric(this Type type, Type interfaceType)
//		{
//			IEnumerable<Type> source = interfaceType.IsInterface
//			                           	?
//			                           		type.GetGenericInterfacesFor(interfaceType) :
//			                           		                                            	type.GetGenericFor(interfaceType);
//
//			foreach (var generic in source)
//			{
//				foreach (var arg in generic.GetGenericArguments())
//				{
//					yield return arg;
//				}
//			}
//		}
//
//		private static IEnumerable<Type> GetGenericInterfacesFor(this Type type, Type interfaceType)
//		{
//			var candidates = type.GetInterfaces();
//			foreach (var candidate in candidates)
//			{
//				if (!candidate.IsGenericType)
//				{
//					continue;
//				}
//
//				var definition = candidate.GetGenericTypeDefinition();
//				if (definition == interfaceType)
//				{
//					yield return candidate;
//				}
//			}
//		}
//
//		private static IEnumerable<Type> GetGenericFor(this Type type, Type targetType)
//		{
//			var baseType = type;
//			while (baseType != null)
//			{
//				if (baseType.IsGenericType)
//				{
//					if (baseType.GetGenericTypeDefinition() == targetType)
//						yield return baseType;
//				}
//
//				baseType = baseType.BaseType;
//			}
//		}

	}
}