namespace Magnum.Core.Tests
{
	using System;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public abstract class Specification
	{
		#region Setup/Teardown

		[SetUp]
		public void SpecificationSetUp()
		{
			_mocks = new MockRepository();
			Before_each_specification();
		}

		[TearDown]
		public void SpecificationTearDown()
		{
			_mocks = null;
			After_each_specification();
		}

		#endregion

		private MockRepository _mocks;

		protected virtual void Before_each_specification()
		{
		}

		protected virtual void After_each_specification()
		{
		}

		protected TMock Mock<TMock>()
		{
			return _mocks.DynamicMock<TMock>();
		}

		protected TStub Stub<TStub>()
		{
			return _mocks.Stub<TStub>();
		}

		protected IDisposable Record
		{
			get { return _mocks.Record(); }
		}

		protected IDisposable Playback
		{
			get { return _mocks.Playback(); }
		}
	}
}