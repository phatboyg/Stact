namespace Magnum.Specs.Pipeline
{
    using System.Diagnostics;
    using Magnum.Pipeline;
    using Magnum.Pipeline.Segments;
    using Magnum.Pipeline.Visitors;
    using Messages;
    using NUnit.Framework;

    [TestFixture]
    public class Examples
    {
        private Pipe _eventAggregator;
        private ISubscriptionScope _scope;

        [SetUp]
        public void Setup()
        {
        }

        [TearDown]
        public void Teardown()
        {
            if(_scope != null)
            {
                _scope.Dispose();
                _scope = null;
            }

            _eventAggregator = null;
        }

        [Test]
        public void First_example()
        {
            _eventAggregator = PipeSegment.Input(PipeSegment.End());

            _eventAggregator.Send(new CustomerRatingDowngraded());
        }

        [Test]
        public void Second_example()
        {
            _eventAggregator = PipeSegment.Input(PipeSegment.End());

            _scope = _eventAggregator.NewSubscriptionScope();
            _scope.Subscribe<CustomerChanged>(message => Trace.WriteLine("Customer changed: " + message.CustomerName));

            new TracePipeVisitor().Trace(_eventAggregator);
        }
        
    }
}