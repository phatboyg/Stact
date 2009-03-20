namespace Magnum.ProtocolBuffers.Specs
{
    using MbUnit.Framework;
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

        protected TMock GenerateMock<TMock>() where TMock : class
        {
            return MockRepository.GenerateMock<TMock>();
        }

        protected TStub GenerateStub<TStub>() where TStub : class
        {
            return MockRepository.GenerateStub<TStub>();
        }
    }

    public static class SpecExtensions
    {
        public static void ShouldEqual(this int actual, int expected)
        {
            Assert.AreEqual(expected, actual);
        }
        public static void ShouldBeTrue(this bool actual)
        {
            Assert.IsTrue(actual);
        }
        public static void ShouldBeFalse(this bool actual)
        {
            Assert.IsFalse(actual);
        }

        public static void ShouldEqual(this string actual, string expected)
        {
            Assert.AreEqual(expected, actual);
        }

        public static void ShouldEqual(this object actual, object expected)
        {
            Assert.AreEqual(expected, actual);
        }
        
    }
}