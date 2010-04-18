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
namespace Magnum.Specs.Extensions
{
	using System;
	using Magnum.Channels;
	using Magnum.Extensions;
	using NUnit.Framework;
	using TestFramework;

	[TestFixture]
	public class An_object_that_implements_a_generic_interface
	{
		private interface INotGeneric
		{			
		}

		private interface IGeneric<T>
		{
		}

		private class GenericClass : 
			IGeneric<int>,
			INotGeneric
		{
		}

		private class GenericSubClass :
			GenericClass
		{
		}

		private class GenericBaseClass<T> :
			IGeneric<T>
		{
			
		}

		private class NonGenericSubClass :
			GenericBaseClass<int>
		{	
		}

		[Test]
		public void Should_match_a_regular_interface_using_the_generic_argument()
		{
			typeof (GenericClass).Implements<INotGeneric>().ShouldBeTrue();
		}

		[Test]
		public void Should_match_a_regular_interface_using_the_type_argument()
		{
			typeof (GenericClass).Implements(typeof (INotGeneric)).ShouldBeTrue();
		}

		[Test]
		public void Should_match_a_regular_interface_on_an_object()
		{
			new GenericClass().Implements<INotGeneric>().ShouldBeTrue();
		}

		[Test]
		public void Should_match_a_regular_interface_by_type_argument_on_an_object()
		{
			new GenericClass().Implements(typeof (INotGeneric)).ShouldBeTrue();
		}

		[Test]
		public void Should_match_a_regular_interface_using_the_generic_argument_on_a_subclass()
		{
			typeof (GenericSubClass).Implements<INotGeneric>().ShouldBeTrue();
		}

		[Test]
		public void Should_match_a_regular_interface_using_the_type_argument_on_a_subclass()
		{
			typeof(GenericSubClass).Implements(typeof(INotGeneric)).ShouldBeTrue();
		}

		[Test]
		public void Should_not_match_a_regular_interface_that_is_not_implemented()
		{
			typeof (GenericClass).Implements<IDisposable>().ShouldBeFalse();
		}

		[Test]
		public void Should_match_a_generic_interface()
		{
			typeof (GenericClass).Implements<IGeneric<int>>().ShouldBeTrue();
		}

		[Test]
		public void Should_match_an_open_generic_interface()
		{
			typeof (GenericClass).Implements(typeof (IGeneric<>)).ShouldBeTrue();
		}

		[Test]
		public void Should_match_an_open_generic_interface_in_a_base_class()
		{
			typeof (NonGenericSubClass).Implements(typeof (IGeneric<>)).ShouldBeTrue();
		}

		[Test]
		public void Should_match_a_generic_base_class_implementation_of_the_interface()
		{
			typeof (NonGenericSubClass).Implements<IGeneric<int>>().ShouldBeTrue();
		}

		[Test]
		public void Should_not_match_a_generic_interface_that_is_not_implemented()
		{
			typeof (NonGenericSubClass).Implements(typeof (Channel<int>)).ShouldBeFalse();
		}
	}
}