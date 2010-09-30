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
namespace Stact.Specs.Reflection
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Stact.Reflection;
	using NUnit.Framework;

	[TestFixture]
	public class Reflecting_over_a_generic_type
	{
		private class SingleNestedGeneric :
			List<string>
		{
		}

		private class DeepSingleNestedGeneric :
			SingleNestedGeneric
		{
		}

		private class DoubleNestedGeneric :
			Dictionary<int, string>
		{
		}

		private class DeepDoubleNestedGeneric :
			DoubleNestedGeneric
		{
		}

		private class SingleGenericInterface :
			ISingleGeneric<int>
		{
		}

		private interface ISingleGeneric<T>
		{
		}

		private class DoubleGenericInterface :
			IDoubleGeneric<int, string>
		{
		}

		private interface IDoubleGeneric<T, K>
		{
		}

		private class NestedDoubleGenericInterface :
			INestedDoubleGeneric<SingleGenericInterface, int>
		{
		}

		private interface INestedDoubleGeneric<T, K>
			where T : ISingleGeneric<K>
		{
		}

		[Test]
		public void Should_return_an_enumeration_of_a_constraint_based_generic_interface()
		{
			var subject = new NestedDoubleGenericInterface();

			Type[] types = subject.GetDeclaredGenericArguments().ToArray();

			Assert.AreEqual(2, types.Length);
			Assert.AreEqual(typeof (SingleGenericInterface), types[0]);
			Assert.AreEqual(typeof (int), types[1]);
		}

		[Test]
		public void Should_return_an_enumeration_of_a_deep_double_nested_generic_type()
		{
			var subject = new DeepDoubleNestedGeneric();

			Type[] types = subject.GetDeclaredGenericArguments().ToArray();

			Assert.AreEqual(2, types.Length);
			Assert.AreEqual(typeof (int), types[0]);
			Assert.AreEqual(typeof (string), types[1]);
		}

		[Test]
		public void Should_return_an_enumeration_of_a_deep_single_nested_generic_type()
		{
			var subject = new DeepSingleNestedGeneric();

			Type[] types = subject.GetDeclaredGenericArguments().ToArray();

			Assert.AreEqual(1, types.Length);
			Assert.AreEqual(typeof (string), types[0]);
		}

		[Test]
		public void Should_return_an_enumeration_of_a_double_generic_interface()
		{
			var subject = new DoubleGenericInterface();

			Type[] types = subject.GetDeclaredGenericArguments().ToArray();

			Assert.AreEqual(2, types.Length);
			Assert.AreEqual(typeof (int), types[0]);
			Assert.AreEqual(typeof (string), types[1]);
		}

		[Test]
		public void Should_return_an_enumeration_of_a_double_generic_type()
		{
			var subject = new Dictionary<int, string>();

			Type[] types = subject.GetDeclaredGenericArguments().ToArray();

			Assert.AreEqual(2, types.Length);
			Assert.AreEqual(typeof (int), types[0]);
			Assert.AreEqual(typeof (string), types[1]);
		}

		[Test]
		public void Should_return_an_enumeration_of_a_double_nested_generic_type()
		{
			var subject = new DoubleNestedGeneric();

			Type[] types = subject.GetDeclaredGenericArguments().ToArray();

			Assert.AreEqual(2, types.Length);
			Assert.AreEqual(typeof (int), types[0]);
			Assert.AreEqual(typeof (string), types[1]);
		}

		[Test]
		public void Should_return_an_enumeration_of_a_single_generic_interface()
		{
			var subject = new SingleGenericInterface();

			Type[] types = subject.GetDeclaredGenericArguments().ToArray();

			Assert.AreEqual(1, types.Length);
			Assert.AreEqual(typeof (int), types[0]);
		}

		[Test]
		public void Should_return_an_enumeration_of_a_single_generic_type()
		{
			var subject = new List<string>();

			Type[] types = subject.GetDeclaredGenericArguments().ToArray();

			Assert.AreEqual(1, types.Length);
			Assert.AreEqual(typeof (string), types[0]);
		}

		[Test]
		public void Should_return_an_enumeration_of_a_single_nested_generic_type()
		{
			var subject = new SingleNestedGeneric();

			Type[] types = subject.GetDeclaredGenericArguments().ToArray();

			Assert.AreEqual(1, types.Length);
			Assert.AreEqual(typeof (string), types[0]);
		}
	}
}