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
namespace Magnum.Specs.Reflection
{
	using System;
	using System.Linq.Expressions;
	using System.Reflection;
	using MbUnit.Framework;

	[TestFixture]
	public class When_invoking_a_generic_method
	{
		[Test]
		public void Invoking_the_method_directly_should_pass_the_appropriate_type()
		{
			MyMethod(new MyClass());
		}

		[Test]
		public void Invoking_with_an_object_should_not_properly_initialize_T()
		{
			object obj = ObjectBuilder.New(typeof (MyClass));

			MyOtherMethod(obj);
		}

		[Test]
		public void Invoking_using_the_generic_method_invoker_should_pass_the_appropriate_type()
		{
			object obj = ObjectBuilder.New(typeof(MyClass));

			GenericMethodInvoker.Invoke(x => MyMethod(x), obj);
		}

		public void MyMethod<T>(T obj)
		{
			obj.GetType().ShouldEqual(typeof (MyClass));
			typeof (T).ShouldEqual(typeof (MyClass));
		}

		public void MyOtherMethod<T>(T obj)
		{
			obj.GetType().ShouldEqual(typeof (MyClass));
			typeof (T).ShouldEqual(typeof (object));
		}

		public class MyClass
		{
		}
	}

	public static class GenericMethodInvoker
	{
		public static void Invoke(Expression<Action<object>> method, object obj)
		{
			MethodCallExpression methodCall = method.Body as MethodCallExpression;
			if(methodCall == null)
				throw new InvalidOperationException("Must be a method call in the expression");

			MethodInfo genericMethodDefinition = methodCall.Method.GetGenericMethodDefinition();
			if(genericMethodDefinition != null)
			{
				MethodInfo methodInfo = genericMethodDefinition.MakeGenericMethod(obj.GetType());

				ConstantExpression e = methodCall.Object as ConstantExpression;

				ParameterExpression instance = Expression.Parameter(typeof(object), "instance");
				ParameterExpression input = Expression.Parameter(typeof (object), "input");

				UnaryExpression instanceCast = Expression.TypeAs(instance, e.Value.GetType());
				UnaryExpression valueCast = Expression.TypeAs(input, obj.GetType());


				var call = Expression.Call(instanceCast, methodInfo, valueCast);

				Action<object,object> invoker = Expression.Lambda<Action<object,object>>(call, new[] {instance, input}).Compile();

				invoker(e.Value, obj);
				return;
			}

			throw new InvalidOperationException("No idea");
		}
	}
}