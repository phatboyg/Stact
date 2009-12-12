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
namespace Magnum.Invoker
{
	using System;
	using System.Linq;
	using System.Reflection;
	using Activator;

	public static class FastInvokerExtensions
	{
		public static void FastInvoke<T>(this T target, string methodName, params object[] args)
		{
			MethodInfo method = typeof (T).GetMethods()
				.Where(x => x.Name == methodName)
				.MatchingArguments(args)
				.First()
				.ToSpecializedMethod(args);

			Func<T, object[], object> callback = FastInvokerFactory.Create<T>(method);

			callback(target, args);
		}
	}
}