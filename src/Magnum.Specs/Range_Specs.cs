namespace Magnum.Common.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using DateTimeExtensions;
    using MbUnit.Framework;

    [TestFixture]
    public class When_a_range_is_specified
    {
        [Test]
        public void It_should_have_the_appropriate_lower_and_upper_bounds()
        {
            Range<int> range = 1.Through(10);

            Assert.AreEqual(1, range.LowerBound);
            Assert.AreEqual(10, range.UpperBound);
            //Assert.That(range.LowerBound, Is.EqualTo(1));
            //Assert.That(range.UpperBound, Is.EqualTo(10));
        }

        [Test]
        public void It_should_be_enumerable()
        {
            Range<int> range = 1.Through(3);

            List<int> values = new List<int>(range.Forward(step => step + 1));

            Assert.AreEqual(3, values.Count);
            //Assert.That(values.Count, Is.EqualTo(3));
        }

        [Test]
        public void It_should_contain_the_lower_bound()
        {
            Range<int> range = 1.Through(7);

            Assert.IsTrue(range.Contains(1));
            //Assert.That(range.Contains(1));
        }

        [Test]
        public void It_should_contain_the_upper_bound()
        {
            Range<int> range = 1.Through(7);

            Assert.IsTrue(range.Contains(7));
            //Assert.That(range.Contains(7));
        }

        [Test]
        public void It_should_allow_LINQ_style_queries()
        {
            Range<int> range = 1.Through(10);

            Range<int> exclude = 5.Through(7);

            Assert.AreEqual(10, range.Forward(step => step + 1).Count());
            //Assert.That(range.Forward(step => step + 1).Count(), Is.EqualTo(10));

            foreach (int value in range.Forward(step => step + 1))
            {
                Debug.WriteLine(value.ToString());
            }

            Assert.AreEqual(3, exclude.Forward(step => step + 1).Count());
            //Assert.That(exclude.Forward(step => step + 1).Count(), Is.EqualTo(3));

            foreach (var value in exclude.Forward(step => step + 1))
            {
                Debug.WriteLine(value.ToString());
            }

            foreach (int value in range.Forward(step => step + 1).Where(x => !exclude.Contains(x)))
            {
                Debug.WriteLine(value.ToString());
				
            }

            Assert.AreEqual(7, range.Forward(step => step + 1).Where(x => !exclude.Contains(x)).Count());
            //Assert.That(range.Forward(step => step + 1).Where(x => !exclude.Contains(x)).Count(), Is.EqualTo(7));
        }

        [Test]
        public void It_should_be_able_to_enumerate_days()
        {
            Range<DateTime> range = DateTime.Now.Through(5.Days().FromNow());

            Assert.AreEqual(6, range.Forward(step => step + 1.Days()).Count());
            //Assert.That(range.Forward(step => step + 1.Days()).Count(), Is.EqualTo(6));
        }

        [Test]
        public void It_should_have_contains_working()
        {
            Range<DateTime> range = DateTime.Now.Through(5.Days().FromNow());

            Assert.IsTrue(range.Contains(DateTime.Now.Next(DayOfWeek.Friday)));
            //Assert.That(range.Contains(DateTime.Now.Next(DayOfWeek.Friday)));
        }
    }
}