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
	}
}