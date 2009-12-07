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
namespace Magnum.Specs.Invoker
{
	using System;
	using System.Linq.Expressions;
	using Classes;
	using NUnit.Framework;

	[TestFixture]
	public class FastInvoker_Specs
	{
		[Test]
		public void FirstTestName()
		{
			var target = new ClassWithSimpleMethods();

			target.FastInvoke(x => x.NoArguments());

			target.NoArgumentsCalled.ShouldBeTrue();
		}

		[Test]
		public void The_generic_method_should_be_invoked()
		{
			var target = new ClassWithGenericMethods();

			object name = "Name";

			target.FastInvoke(x => x.OneGenericArgument(0), name);

		}
	}

	public static class FastInvokeExtension
	{
		public static void FastInvoke<T>(this T target, Expression<Action<T>> expression)
		{
		}

		public static void FastInvoke<T>(this T target, Expression<Action<T>> expression, object arg0)
		{
		}
	}
}
