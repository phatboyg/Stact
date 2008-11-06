namespace Magnum.ProtocolBuffers.Specs
{
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class Specification
    {
        #region Setup/Teardown

        [SetUp]
        public void SpecificationSetUp()
        {
            Before_each();
        }

        [TearDown]
        public void SpecificationTearDown()
        {
            After_each();
        }

        #endregion

        protected virtual void Before_each()
        {
        }

        protected virtual void After_each()
        {
        }

        protected TMock GenerateMock<TMock>()
        {
            return MockRepository.GenerateMock<TMock>();
        }

        protected TStub GenerateStub<TStub>()
        {
            return MockRepository.GenerateStub<TStub>();
        }
    }
}