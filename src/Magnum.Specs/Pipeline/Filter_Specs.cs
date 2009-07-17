namespace Magnum.Specs.Pipeline
{
    using System;
    using Magnum.Actors;
    using Magnum.Pipeline;
    using Magnum.Pipeline.Segments;
    using NUnit.Framework;

    [TestFixture]
    public class Connecting_a_filter_to_the_pipeline
    {
        [Test]
        public void Should_pass_derived_types_through_the_filter()
        {
            var received = new Future<bool>();

            Pipe consumer = PipeSegment.Consumer<BaseClass>(x => received.Complete(true));
            Pipe filter = PipeSegment.Filter<object>(consumer);

            filter.Send(new SubClass());

            received.IsAvailable(TimeSpan.Zero).ShouldBeTrue();
        }

        [Test]
        public void Should_not_pass_unwanted_types_through_the_filter()
        {
            var received = new Future<bool>();

            Pipe consumer = PipeSegment.Consumer<SubClass>(x => received.Complete(true));
            Pipe filter = PipeSegment.Filter<object>(consumer);

            filter.Send(new BaseClass());

            received.IsAvailable(TimeSpan.Zero).ShouldBeFalse();
        }

        private class BaseClass
        {
        }

        private class SubClass : 
            BaseClass
        {
        }
    }
}