namespace Magnum.Common.Specs
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using ObjectExtensions;

	[TestFixture]
    public class When_a_value_is_being_verified
    {
        [Test]
        public void An_exception_should_be_thrown_if_the_value_is_out_of_range()
        {
            List<int> items = new List<int> { 1,2,3,4,5,6,7};
            int value = 27;

            try
            {
                value.MustBeInRange(0.Through(items.Count));

                Assert.Fail("The exception should have been thrown");
            }
            catch (ArgumentException)
            {
            }
            catch (Exception)
            {
                Assert.Fail("Should have been an ArgumentException");
            }
        }
    }
}