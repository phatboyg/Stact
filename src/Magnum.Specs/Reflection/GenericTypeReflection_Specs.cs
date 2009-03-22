namespace Magnum.Specs.Reflection
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using MbUnit.Framework;
	using Magnum.Reflection;

	[TestFixture]
	public class Reflecting_over_a_generic_type
	{
		[Test]
		public void Should_return_an_enumeration_of_a_single_generic_type()
		{
			var subject = new List<string>();

			var types = subject.GetDeclaredGenericArguments().ToArray();

			Assert.AreEqual(1, types.Length);
			Assert.AreEqual(typeof (string), types[0]);
		}

		[Test]
		public void Should_return_an_enumeration_of_a_single_nested_generic_type()
		{
			var subject = new SingleNestedGeneric();

			var types = subject.GetDeclaredGenericArguments().ToArray();

			Assert.AreEqual(1, types.Length);
			Assert.AreEqual(typeof (string), types[0]);
		}

		[Test]
		public void Should_return_an_enumeration_of_a_deep_single_nested_generic_type()
		{
			var subject = new DeepSingleNestedGeneric();

			var types = subject.GetDeclaredGenericArguments().ToArray();

			Assert.AreEqual(1, types.Length);
			Assert.AreEqual(typeof (string), types[0]);
		}

		[Test]
		public void Should_return_an_enumeration_of_a_double_generic_type()
		{
			var subject = new Dictionary<int, string>();

			var types = subject.GetDeclaredGenericArguments().ToArray();

			Assert.AreEqual(2, types.Length);
			Assert.AreEqual(typeof (int), types[0]);
			Assert.AreEqual(typeof (string), types[1]);
		}

		[Test]
		public void Should_return_an_enumeration_of_a_double_nested_generic_type()
		{
			var subject = new DoubleNestedGeneric();

			var types = subject.GetDeclaredGenericArguments().ToArray();

			Assert.AreEqual(2, types.Length);
			Assert.AreEqual(typeof (int), types[0]);
			Assert.AreEqual(typeof (string), types[1]);
		}

		[Test]
		public void Should_return_an_enumeration_of_a_deep_double_nested_generic_type()
		{
			var subject = new DeepDoubleNestedGeneric();

			var types = subject.GetDeclaredGenericArguments().ToArray();

			Assert.AreEqual(2, types.Length);
			Assert.AreEqual(typeof (int), types[0]);
			Assert.AreEqual(typeof (string), types[1]);
		}

		private class SingleNestedGeneric :
			List<string>
		{
		}

		private class DeepSingleNestedGeneric :
			SingleNestedGeneric
		{
		}

		private class DoubleNestedGeneric :
			Dictionary<int,string>
		{
		}

		private class DeepDoubleNestedGeneric :
			DoubleNestedGeneric
		{
		}

		[Test]
		public void Should_return_an_enumeration_of_a_single_generic_interface()
		{
			var subject = new SingleGenericInterface();

			var types = subject.GetDeclaredGenericArguments().ToArray();

			Assert.AreEqual(1, types.Length);
			Assert.AreEqual(typeof(int), types[0]);
		}

		private class SingleGenericInterface :
			ISingleGeneric<int>
		{
		}

		private interface ISingleGeneric<T>
		{
		}

		[Test]
		public void Should_return_an_enumeration_of_a_double_generic_interface()
		{
			var subject = new DoubleGenericInterface();

			var types = subject.GetDeclaredGenericArguments().ToArray();

			Assert.AreEqual(2, types.Length);
			Assert.AreEqual(typeof(int), types[0]);
			Assert.AreEqual(typeof(string), types[1]);
		}

		private class DoubleGenericInterface :
			IDoubleGeneric<int,string>
		{
		}

		private interface IDoubleGeneric<T,K>
		{
		}

		[Test]
		public void Should_return_an_enumeration_of_a_constraint_based_generic_interface()
		{
			var subject = new NestedDoubleGenericInterface();

			var types = subject.GetDeclaredGenericArguments().ToArray();

			Assert.AreEqual(2, types.Length);
			Assert.AreEqual(typeof(SingleGenericInterface), types[0]);
			Assert.AreEqual(typeof(int), types[1]);
		}

		private class NestedDoubleGenericInterface :
			INestedDoubleGeneric<SingleGenericInterface, int>
		{
		}

		private interface INestedDoubleGeneric<T,K>
			where T : ISingleGeneric<K>
		{
		}
	}

	[TestFixture]
	public class When_reflecting_a_generic_method
	{
		private bool _invoked;
		private Type[] _argumentTypes;

		[SetUp]
		public void Establish_context()
		{
			_invoked = false;
			_argumentTypes = null;
		}

		[Test]
		public void Should_return_the_generic_types_expected()
		{
			MethodInfo methodInfo = GetType().GetMethodCandidates("SingleGeneric").FirstOrDefault();

			var types = methodInfo.GetGenericArguments().ToArray();

			Assert.AreEqual(1, types.Length);
			Assert.AreEqual("T", types[0].Name);
		}

		[Test]
		public void Should_determine_the_argument_types_to_call_the_method_properly()
		{
			object[] args = new object[] { 27 };
			var argumentTypes = args.GetElementTypes();

			MethodInfo methodInfo = GetType()
				.GetMethodCandidates("SingleGeneric")
				.FindBestMatch(args);

			methodInfo
				.InvokeOn<object>(this, args);

			Assert.IsTrue(_invoked);
			Assert.IsNotNull(_argumentTypes);
			Assert.AreEqual(1, _argumentTypes.Length);
			Assert.AreEqual(typeof(int), _argumentTypes[0]);
		}

		private void SingleGeneric<T>(T argument)
		{
			_invoked = true;
			_argumentTypes = new[] {typeof (T)};
		}

		[Test]
		public void Should_determine_the_argument_types_via_generic_constraints()
		{
			object[] args = new object[] { new HiddenGeneric() };
			var argumentTypes = args.GetElementTypes();

			MethodInfo methodInfo = GetType()
				.GetMethodCandidates("ComplexGeneric")
				.FindBestMatch(args);

			methodInfo
				.InvokeOn<object>(this, args);

			Assert.IsTrue(_invoked);
			Assert.IsNotNull(_argumentTypes);
			Assert.AreEqual(2, _argumentTypes.Length);
			Assert.AreEqual(typeof(HiddenGeneric), _argumentTypes[0]);
			Assert.AreEqual(typeof(int), _argumentTypes[1]);
		}

		private void ComplexGeneric<T,V>(T argument)
			where T : ISingleGeneric<V>
		{
			_invoked = true;
			_argumentTypes = new[] { typeof(T), typeof(V) };
		}

		private class HiddenGeneric :
			ISingleGeneric<int>
		{
		}

		private interface ISingleGeneric<T>
		{
		}
	}
}