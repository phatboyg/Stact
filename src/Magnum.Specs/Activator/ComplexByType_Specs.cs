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
	public class When_creating_an_object_with_multiple_arguments
	{
		private Type _objectType = typeof (ClassWithMultipleComplexConstructors);

		[Test]
		public void The_first_constructor_should_be_called()
		{
			const string description = "Description";
			const string name = "Name";
			const int count = 21;
			const int value = 47;

			var args = new object[] {value, name, count, description};

			object instance = FastActivator.Create(_objectType, args);

			Assert.IsNotNull(instance);
			Assert.IsInstanceOfType(_objectType, instance);

			var typed = (ClassWithMultipleComplexConstructors) instance;

			typed.Value.ShouldEqual(value);
			typed.Name.ShouldEqual(name);
			typed.Count.ShouldEqual(count);
			typed.Description.ShouldEqual(description);
		}

		[Test]
		public void The_second_constructor_should_be_called()
		{
			const string description = "Description";
			const string name = "Name";
			const int count = 21;
			const int value = 47;

			var args = new object[] {name, value, count, description};

			object instance = FastActivator.Create(_objectType, args);

			Assert.IsNotNull(instance);
			Assert.IsInstanceOfType(_objectType, instance);

			var typed = (ClassWithMultipleComplexConstructors) instance;

			typed.Value.ShouldBeEqualTo(value);
			typed.Name.ShouldEqual(name);
			typed.Count.ShouldEqual(count);
			typed.Description.ShouldEqual(description);
		}
	}
}