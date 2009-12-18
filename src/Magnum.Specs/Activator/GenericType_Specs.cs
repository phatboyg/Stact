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
namespace Magnum.Specs.Activator
{
	using System;
	using Classes;
	using Magnum.Reflection;
	using NUnit.Framework;
	using TestFramework;

	[TestFixture]
	public class When_creating_a_generic_type
	{
		[Test]
		public void The_generic_arguments_should_be_used_to_build_the_type()
		{
			var obj = FastActivator.Create(typeof (ClassWithOneGenericArgument<>), new[] {typeof (int)});

			obj.ShouldNotBeNull();
			obj.ShouldBeAnInstanceOf<ClassWithOneGenericArgument<int>>();
		}

		[Test]
		public void The_generic_type_should_be_inferred_from_the_argument()
		{
			const int value = 27;

			var obj = FastActivator.Create(typeof (ClassWithOneGenericArgument<>), value);

			obj.ShouldNotBeNull();
			obj.ShouldBeAnInstanceOf<ClassWithOneGenericArgument<int>>();

			var instance = (ClassWithOneGenericArgument<int>) obj;

			instance.Value.ShouldEqual(value);
			instance.Count.ShouldEqual(default(int));
		}

		[Test]
		public void The_generic_type_should_be_inferred_from_the_first_argument()
		{
			const string value = "Name";
			const int count = 47;

			var obj = FastActivator.Create(typeof (ClassWithOneGenericArgument<>), value, count);

			obj.ShouldNotBeNull();
			obj.ShouldBeAnInstanceOf<ClassWithOneGenericArgument<string>>();

			var instance = (ClassWithOneGenericArgument<string>) obj;

			instance.Value.ShouldEqual(value);
			instance.Count.ShouldEqual(count);
		}
	}

	[TestFixture]
	public class When_creating_a_generic_class_with_a_constrained_generic_argument
	{
		[Test]
		public void An_object_matching_the_constraint_should_properly_define_the_generic_type()
		{
			Guid id = CombGuid.Generate();

			var argument = new ClassWithGuidConstraint(id);

			var obj = FastActivator.Create(typeof (ClassWithAConstrainedGenericArgument<,>), argument);

			obj.ShouldNotBeNull();
			obj.ShouldBeAnInstanceOf<ClassWithAConstrainedGenericArgument<ClassWithGuidConstraint, Guid>>();

			var instance = (ClassWithAConstrainedGenericArgument<ClassWithGuidConstraint, Guid>) obj;

			instance.Value.ShouldEqual(argument);
		}

		[Test, ExpectedException(typeof(FastActivatorException))]
		public void An_object_not_matching_the_constraint_should_throw_an_exception()
		{
			var argument = new ClassWithDefaultConstructor();

			var obj = FastActivator.Create(typeof (ClassWithAConstrainedGenericArgument<,>), argument);

			Assert.Fail("Should not have succeeded");
		}

		[Test]
		public void An_object_matching_the_constraint_should_properly_define_the_generic_type_with_deep()
		{
			Guid id = CombGuid.Generate();

			var argument = new SuperConstrainedClass(id);

			var obj = FastActivator.Create(typeof (ClassWithAConstrainedGenericArgument<,>), argument);

			obj.ShouldNotBeNull();
			obj.ShouldBeAnInstanceOf<ClassWithAConstrainedGenericArgument<SuperConstrainedClass, Guid>>();

			var instance = (ClassWithAConstrainedGenericArgument<SuperConstrainedClass, Guid>) obj;

			instance.Value.ShouldEqual(argument);
		}

		[Test]
		public void An_object_with_multiple_constraints_should_be_created()
		{
			Guid id = CombGuid.Generate();
			var argument = new SuperConstrainedClass(id);

			string name = "Name";
			var argument2 = new ClassWithStringConstraint(name);

			var obj = FastActivator.Create(typeof (ClassWithTwoConstrainedGenericArguments<,,,>), argument, argument2);

			obj.ShouldNotBeNull();
			obj.ShouldBeAnInstanceOf<ClassWithTwoConstrainedGenericArguments<SuperConstrainedClass, Guid, ClassWithStringConstraint, string>>();

			var instance = (ClassWithTwoConstrainedGenericArguments<SuperConstrainedClass, Guid, ClassWithStringConstraint, string>) obj;

			instance.Value.ShouldEqual(argument);
			instance.Value2.ShouldEqual(argument2);
		}
	}
}