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
namespace Stact.Specs.Extensions
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Stact.Extensions;
	using NUnit.Framework;
	using TestFramework;

	[TestFixture]
	public class When_getting_the_generic_types_from_an_interface
	{
		private interface IGeneric<T>
		{
		}

		private class GenericClass :
			IGeneric<int>
		{
		}

		private class SubClass :
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
		public void Should_return_the_appropriate_generic_type()
		{
			IEnumerable<Type> types = typeof (GenericClass).GetGenericTypeDeclarations(typeof (IGeneric<>));

			types.Count().ShouldEqual(1);
			types.First().ShouldEqual(typeof (int));
		}

		[Test]
		public void Should_return_the_appropriate_generic_type_for_a_subclass_non_generic()
		{
			IEnumerable<Type> types = typeof (SubClass).GetGenericTypeDeclarations(typeof (IGeneric<>));

			types.Count().ShouldEqual(1);
			types.First().ShouldEqual(typeof (int));
		}

		[Test]
		public void Should_return_the_appropriate_generic_type_with_a_generic_base_class()
		{
			IEnumerable<Type> types = typeof(NonGenericSubClass).GetGenericTypeDeclarations(typeof(IGeneric<>));

			types.Count().ShouldEqual(1);
			types.First().ShouldEqual(typeof (int));
		}

		[Test]
		public void Should_return_the_generic_type_from_a_class()
		{
			IEnumerable<Type> types = typeof(NonGenericSubClass).GetGenericTypeDeclarations(typeof(GenericBaseClass<>));

			types.Count().ShouldEqual(1);
			types.First().ShouldEqual(typeof (int));
		}
	}
}