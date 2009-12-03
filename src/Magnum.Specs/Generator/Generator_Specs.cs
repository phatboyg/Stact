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
namespace Magnum.Specs.Generator
{
	using Classes;
	using Magnum.Generator;
	using NUnit.Framework;

	[TestFixture]
	public class When_generating_an_object<T>
		where T : class
	{
		protected T Instance;

		[SetUp]
		public void Generating_an_object()
		{
			Instance = ObjectGenerator<T>.Create();
		}

		[Test]
		public void The_object_should_not_be_null()
		{
			Assert.IsNotNull(Instance);
		}

		[Test]
		public void The_object_should_be_of_the_requested_type()
		{
			Assert.IsInstanceOfType(typeof (T), Instance);
		}
	}

	public class When_generating_an_object_by_type<T>
		where T : class
	{
		protected T Instance;

		[SetUp]
		public void Generating_an_object()
		{
			Instance = ObjectGenerator.Create(typeof(T)) as T;
		}

		[Test]
		public void The_object_should_not_be_null()
		{
			Assert.IsNotNull(Instance);
		}

		[Test]
		public void The_object_should_be_of_the_requested_type()
		{
			Assert.IsInstanceOfType(typeof (T), Instance);
		}
	}

	[TestFixture]
	public class When_generating_a_simple_object_with_a_default_constructor_by_type :
		When_generating_an_object_by_type<ClassWithDefaultConstructor>
	{
		[Test]
		public void The_field_should_be_initialized()
		{
			Assert.AreEqual(47, Instance.Value);
		}
	}

	[TestFixture]
	public class When_generating_a_simple_object_with_the_default_constructor :
		When_generating_an_object<ClassWithDefaultConstructor>
	{
		[Test]
		public void The_field_should_be_initialized()
		{
			Assert.AreEqual(47, Instance.Value);
		}
	}

	[TestFixture]
	public class When_generating_a_simple_object_with_a_private_constructor :
		When_generating_an_object<ClassWithPrivateConstructor>
	{
		[Test]
		public void The_field_should_be_initialized()
		{
			Assert.AreEqual(47, Instance.Value);
		}
	}

	[TestFixture]
	public class When_generating_a_simple_object_with_a_protected_constructor :
		When_generating_an_object<ClassWithProtectedConstructor>
	{
		[Test]
		public void The_field_should_be_initialized()
		{
			Assert.AreEqual(47, Instance.Value);
		}
	}
}