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
	using System.Collections.Generic;
	using System.Diagnostics;
	using Magnum.Activator;
	using Magnum.Reflection;
	using NUnit.Framework;

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
			object obj = FastActivator.Create(typeof (MyClass));

			MyOtherMethod(obj);
		}

		[Test]
		public void Invoking_using_the_generic_method_invoker_should_pass_the_appropriate_type()
		{
			object obj = FastActivator.Create(typeof(MyClass));

			Generic.Call(x => MyMethod(x), obj);
		}

		[Test, Ignore] // okay, static doesn't seem tow ork after all
		public void A_static_method_should_also_be_able_to_be_invoked()
		{
			object obj = FastActivator.Create(typeof(MyClass));

			Generic.Call(x => MyStaticMethod(x), obj);
		}

		[Test]
		public void The_extension_method_syntax_should_work_the_same()
		{
			object obj = FastActivator.Create(typeof(MyClass));

			this.Call("MyMethod", obj);
		}

		[Test]
		public void The_extension_method_syntax_should_work_the_same_for_a_static_method()
		{
			object obj = FastActivator.Create(typeof(MyClass));

			this.Call("MyStaticMethod", obj);
		}

		[Test]
		public void Okay_now_we_have_a_strange_problem_here()
		{
			string key = "Chris";
			MyClass value = FastActivator<MyClass>.Create();

			var pair = new KeyValuePair<string,MyClass>(key, value);

			this.Call("ComplexMethod", "ignored", pair);
		}


		[Test]
		public void Invoking_using_the_generic_method_invoker_should_pass_the_appropriate_type_for_value_types()
		{
			Generic.Call(x => MyIntMethod(x), 27);
		}

		[Test, ExpectedException(typeof(InvalidOperationException))]
		public void Invoking_a_regular_method_should_not_work_too()
		{
			object obj = FastActivator.Create(typeof(MyClass));

			Generic.Call(x => RegularMethod(x), obj);
		}



		[Test]
		public void Invoking_it_a_lot_should_be_fast()
		{
			object obj = FastActivator.Create(typeof(MyClass));

			Generic.Call(x => MyMethod(x), obj);

			Stopwatch count = Stopwatch.StartNew();
			for (int i = 0; i < 10000; i++)
			{
				Generic.Call(x => MyMethod(x), obj);
			}
			count.Stop();

			Console.WriteLine("time to run = " + count.ElapsedMilliseconds + "ms");
		}

		public void MyMethod<T>(T obj)
		{
			obj.GetType().ShouldEqual(typeof (MyClass));
			typeof (T).ShouldEqual(typeof (MyClass));
		}

		public static void MyStaticMethod<T>(T obj)
		{
			obj.GetType().ShouldEqual(typeof (MyClass));
			typeof (T).ShouldEqual(typeof (MyClass));
		}

		public void MyIntMethod<T>(T obj)
		{
			obj.GetType().ShouldEqual(typeof (int));
			typeof (T).ShouldEqual(typeof (int));
		}

		public void MyOtherMethod<T>(T obj)
		{
			obj.GetType().ShouldEqual(typeof (MyClass));
			typeof (T).ShouldEqual(typeof (object));
		}

		public void RegularMethod(object obj)
		{
		}

		public void ComplexMethod<TKey,TValue>(string ignored, KeyValuePair<TKey,TValue> pair)
		{
			pair.Key.GetType().ShouldEqual(typeof (string));
			pair.Value.GetType().ShouldEqual(typeof (MyClass));

			typeof (TKey).ShouldEqual(typeof (string));
			typeof (TValue).ShouldEqual(typeof (MyClass));
		}

		public class MyClass
		{
		}


	}
}