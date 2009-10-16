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
	using Magnum.Reflection;
	using NUnit.Framework;

	[TestFixture]
	public class When_creating_an_object_using_the_object_builder
	{
		[Test]
		public void It_should_be_able_to_create_objects_with_only_the_default_constructor()
		{
			object obj = ClassFactory.New(typeof (TestClassWithDefaultConstructor));

			obj.ShouldNotBeNull();
		}

		[Test]
		public void It_should_be_able_to_create_objects_with_private_constructors()
		{
			object obj = ClassFactory.New(typeof(TestClassWithPrivateConstructor));

			obj.ShouldNotBeNull();
		}

		[Test]
		public void It_should_be_able_to_create_generic_objects()
		{
			object captured = ClassFactory.New(typeof (TestClassWithDefaultConstructor));

			object obj = ClassFactory.New(typeof (TestGenericClass<>), captured);

			obj.ShouldNotBeNull();

			obj.GetType().ShouldEqual(typeof (TestGenericClass<TestClassWithDefaultConstructor>));

			var typedObj = obj as TestGenericClass<TestClassWithDefaultConstructor>;

			typedObj.ShouldNotBeNull();

			typedObj.Captured.ShouldEqual(captured);

			typedObj.Value.ShouldEqual(0);
		}

		[Test]
		public void It_should_be_able_to_create_generic_objects_with_multiple_arguments()
		{
			object captured = ClassFactory.New(typeof(TestClassWithDefaultConstructor));

			int value = 27;
			object obj = ClassFactory.New(typeof(TestGenericClass<>), captured, value);

			obj.ShouldNotBeNull();

			obj.GetType().ShouldEqual(typeof(TestGenericClass<TestClassWithDefaultConstructor>));

			var typedObj = obj as TestGenericClass<TestClassWithDefaultConstructor>;

			typedObj.ShouldNotBeNull();

			typedObj.Captured.ShouldEqual(captured);

			typedObj.Value.ShouldEqual(value);
		}

		[Test]
		public void A_double_generic_should_be_creatable_()
		{
			object correlated = ClassFactory.New(typeof (CorrelatedClass));
			correlated.ShouldNotBeNull();

			object bing = ClassFactory.New(typeof (CapturedGeneric<,>), correlated);
			bing.ShouldNotBeNull();

			bing.GetType().ShouldEqual(typeof(CapturedGeneric<CorrelatedClass,Guid>));
		}

		[Test]
		public void A_class_with_an_argument_should_be_creatable()
		{
			Guid guid = Guid.NewGuid();
			var obj = ClassFactory.New<TestGuidConstructor>(guid);
			obj.ShouldNotBeNull();

			obj.CorrelationId.ShouldEqual(guid);
		}


		public class TestGuidConstructor
		{
			public Guid CorrelationId { get; set; }

			public TestGuidConstructor(Guid correlationId)
			{
				CorrelationId = correlationId;
			}
		}


		public class TestClassWithDefaultConstructor
		{
		}

		public class TestClassWithPrivateConstructor
		{
			private TestClassWithPrivateConstructor()
			{
			}
		}

		public class CapturedGeneric<T,K>
			where T : CorrelatedBy<K>
		{
			public CapturedGeneric(T value)
			{
				
			}
			
		}

		public interface CorrelatedBy<T>
		{
			T Id { get; }
		}

		public class CorrelatedClass : 
			CorrelatedBy<Guid>
		{
			public Guid Id
			{
				get { throw new NotImplementedException(); }
			}
		}

		public class TestGenericClass<T>
		{
			private T _captured;
			private readonly int _value;

			public TestGenericClass(T captured)
			{
				_captured = captured;
			}

			public TestGenericClass(T captured, int value)
			{
				_captured = captured;
				_value = value;
			}

			public T Captured
			{
				get { return _captured; }
			}

			public int Value
			{
				get { return _value; }
			}
		}
	}
}