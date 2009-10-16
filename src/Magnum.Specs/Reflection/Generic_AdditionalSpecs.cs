namespace Magnum.Specs.Reflection
{
	using System;
	using NUnit.Framework;
	using Rhino.Mocks;
	using Magnum.Reflection;

	[TestFixture]
	public class ObjectSendExtensionTests
	{

		[Test]
		public void FailsIfTargetIsNull()
		{
			const string x = null;
			//Assert.Throws<ArgumentNullException>(() => x.Call("ToString"));
		}

		[Test]
		public void FailsIfMethodNameIsNull()
		{
			//Assert.Throws<ArgumentNullException>(() => 123.Call(null));
		}

		[Test]
		public void CanInvokeUniqueMethodWithoutArgs()
		{
			var mock = MockRepository.GenerateMock<IUniqueMethodWithoutArgs>();
			mock.Call("Foo");
			mock.AssertWasCalled(x => x.Foo());
		}

		[Test]
		public void CanDistinguishMethodsByParameterCount_FirstMethod()
		{
			var mock = MockRepository.GenerateMock<ISimpleOverload>();
			mock.Call("Foo");
			mock.AssertWasCalled(x => x.Foo());
			mock.AssertWasNotCalled(x => x.Foo(Arg<int>.Is.Anything));
		}

		[Test]
		public void CanDistinguishMethodsByParameterCount_SecondMethod()
		{
			var mock = MockRepository.GenerateMock<ISimpleOverload>();
			mock.Call("Foo", 42);
			mock.AssertWasCalled(x => x.Foo(Arg.Is(42)));
			mock.AssertWasNotCalled(x => x.Foo());
		}

		[Test]
		public void CanPassArgumentsToMethod()
		{
			var mock = MockRepository.GenerateMock<ISimpleArguments>();
			mock.Call("Foo", 4, "bar");
			mock.AssertWasCalled(x => x.Foo(Arg.Is(4), Arg.Is("bar")));
		}

		[Test]
		public void CanDoSimpleParameterResolution_StringOverload()
		{
			var mock = MockRepository.GenerateMock<IParameterResolution>();
			var arg = "bar";
			mock.Call("Foo", arg);
			mock.AssertWasCalled(x => x.Foo(Arg.Is(arg)));
			mock.AssertWasNotCalled(x => x.Foo(Arg<Version>.Is.Anything));
			mock.AssertWasNotCalled(x => x.Foo(Arg<int>.Is.Anything));
		}

		[Test]
		public void CanDoSimpleParameterResolution_VersionOverload()
		{
			var mock = MockRepository.GenerateMock<IParameterResolution>();
			var arg = new Version(1, 2, 3);
			mock.Call("Foo", arg);
			mock.AssertWasCalled(x => x.Foo(Arg.Is(arg)));
			mock.AssertWasNotCalled(x => x.Foo(Arg<string>.Is.Anything));
			mock.AssertWasNotCalled(x => x.Foo(Arg<int>.Is.Anything));
		}

		[Test]
		public void CanDoSimpleParameterResolution_IntOverload()
		{
			var mock = MockRepository.GenerateMock<IParameterResolution>();
			var arg = 42;
			mock.Call("Foo", arg);
			mock.AssertWasCalled(x => x.Foo(Arg.Is(arg)));
			mock.AssertWasNotCalled(x => x.Foo(Arg<string>.Is.Anything));
			mock.AssertWasNotCalled(x => x.Foo(Arg<Version>.Is.Anything));
		}

		[Test]
		public void CanHandleNullableArguments()
		{
			var mock = MockRepository.GenerateMock<INullableParameters>();
			int? arg = 42;
			mock.Call("Foo", arg);
			mock.AssertWasCalled(x => x.Foo(Arg.Is(arg)));
			mock.AssertWasNotCalled(x => x.Foo(Arg<float>.Is.Anything));
		}

		[Test]
		public void CanHandleNullableArgumentsWithNullValue()
		{
			var mock = MockRepository.GenerateMock<INullableParameters>();
			int? arg = null;
			mock.Call("Foo", arg);
			mock.AssertWasCalled(x => x.Foo(Arg.Is(arg)));
			mock.AssertWasNotCalled(x => x.Foo(Arg<float>.Is.Anything));
		}

		[Test, Description("Although not desired this behaviour is expected")]
		public void SuffersFromNullableBoxingBehaviour()
		{
			var mock = MockRepository.GenerateMock<INullableParametersBoxingIssue>();
			int? arg = 42;
			mock.Call("Foo", arg);
			mock.AssertWasCalled(x => x.Foo(Arg<int>.Is.Equal(42)));
			mock.AssertWasNotCalled(x => x.Foo(Arg<int?>.Is.Anything));
		}

		[Test]
		public void TriesToMatchTypesAsGoodAsPossible()
		{
			var mock = MockRepository.GenerateMock<ISelectPolymorphic>();
			var item = new DerivedClass();
			mock.Call("Foo", item);
			mock.AssertWasCalled(x => x.Foo(Arg<DerivedClass>.Is.Same(item)));
			mock.AssertWasNotCalled(x => x.Foo(Arg<BaseClass>.Is.Anything));
		}

		[Test, Ignore]
		public void TriesToMatchTypesAsGoodAsPossible2()
		{
			var mock = MockRepository.GenerateMock<ISelectPolymorphic2>();
			var item = new DerivedClass();
			mock.Call("Foo", null, item);
			mock.AssertWasCalled(x => x.Foo(Arg<BaseClass>.Is.Null, Arg<DerivedClass>.Is.Same(item)));
			mock.AssertWasNotCalled(x => x.Foo(Arg<BaseClass>.Is.Anything, Arg<BaseClass>.Is.Anything));
		}

		[Test]
		public void CanCauseAmbiguousInvocation()
		{
			var mock = MockRepository.GenerateMock<ISelectPolymorphic>();

			//var exception = Assert.Throws<ArgumentException>(() => mock.Call("Foo", new object[] { null }));
			//Assert.AreEqual("Ambiguous method invocation", exception.Message);
		}

		[Test]
		public void CanHandleNullArrayAsArguments()
		{
			var mock = MockRepository.GenerateMock<IUniqueMethodWithoutArgs>();
			mock.Call("Foo", default(object[]));
			mock.AssertWasCalled(x => x.Foo());
		}

		[Test]
		public void ReturnsReturnValue()
		{
			var stub = MockRepository.GenerateStub<IReturnValue>();
			stub.Stub(x => x.IntFoo()).Return(123);
			stub.Stub(x => x.StringFoo()).Return("bar");

			var intResult = stub.Call("IntFoo");
			var stringResult = stub.Call("StringFoo");

			Assert.AreEqual(123, intResult);
			Assert.AreEqual("bar", stringResult);
		}

		[Test]
		public void ReturnsCastReturnValue()
		{
			var stub = MockRepository.GenerateStub<IReturnValue>();
			stub.Stub(x => x.IntFoo()).Return(123);
			stub.Stub(x => x.StringFoo()).Return("bar");

			int intResult = stub.Call<int>("IntFoo");
			string stringResult = stub.Call<string>("StringFoo");

			Assert.AreEqual(123, intResult);
			Assert.AreEqual("bar", stringResult);
		}

		public interface IUniqueMethodWithoutArgs
		{
			void Foo();
		}

		public interface ISimpleOverload
		{
			void Foo();
			void Foo(int x);
		}

		public interface ISimpleArguments
		{
			void Foo(int x, string y);
		}

		public interface IParameterResolution
		{
			void Foo(string x);
			void Foo(Version x);
			void Foo(int x);
		}

		public interface INullableParameters
		{
			void Foo(float a);
			void Foo(int? a);
		}

		public interface INullableParametersBoxingIssue
		{
			void Foo(int a);
			void Foo(int? a);
		}

		public interface ISelectPolymorphic
		{
			void Foo(BaseClass arg);
			void Foo(DerivedClass arg);
		}

		public interface ISelectPolymorphic2
		{
			void Foo(BaseClass arg, BaseClass arg2);
			void Foo(BaseClass arg, DerivedClass arg2);
		}

		public interface IReturnValue
		{
			int IntFoo();
			string StringFoo();
		}

		public class BaseClass
		{
		}

		public class DerivedClass : BaseClass
		{
		}
	}
}