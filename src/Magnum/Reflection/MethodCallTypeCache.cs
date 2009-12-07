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
	using Activator;
	using Collections;
	using ObjectExtensions;

	public class MethodCallTypeCache :
		IMethodCall
	{
		private readonly Cache<Type, IMethodCall> _typeCache = new Cache<Type, IMethodCall>(CreateNewValue);

		public T Call<T>(object instance, string methodName, params object[] args)
		{
			instance.MustNotBeNull("instance");
			methodName.MustNotBeEmpty("methodName");

			Type instanceType = instance.GetType();

			return _typeCache.Retrieve(instanceType).Call<T>(instance, methodName, args);
		}

		public T Call<T>(object instance, string methodName, Type[] argumentTypes, params object[] args)
		{
			instance.MustNotBeNull("instance");
			methodName.MustNotBeEmpty("methodName");

			Type instanceType = instance.GetType();

			return _typeCache.Retrieve(instanceType).Call<T>(instance, methodName, argumentTypes, args);
		}

		private static IMethodCall CreateNewValue(Type key)
		{
			return (IMethodCall) FastActivator.Create(typeof (MethodCallCache<>).MakeGenericType(key));
		}
	}
}