namespace Magnum.Specs.CEP
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using DateTimeExtensions;
    using Magnum.Actors;
    using Magnum.Actors.CommandQueues;
    using NUnit.Framework;

    [TestFixture]
    public class TimeBased_Specs
    {
        [Test]
        public void NAME()
        {
            var o = new object();
            var i = 0;
            var mre = new ManualResetEvent(false);
            var cq = new ThreadCommandQueue(10, 1);
            var q = new TimeBasedQueue<object>(cq, 1.Seconds(), (x)=>
            {
                i++;
            });
            q.Enqueue(o);
            cq.Run();

            Assert.AreEqual(1, i);
        }
    }

    [TestFixture]
    public class TimedEntry_Specs
    {
        [Test]
        public void HasExpiredWorks()
        {
            var t = new TimedEntry<object>(new object(), 1.Seconds());
            Thread.Sleep(1001);
            Assert.IsTrue(t.HasExpired);
        }

        [Test]
        public void TheOppositeTest()
        {
            var t = new TimedEntry<object>(new object(), 1.Seconds());

            Assert.IsFalse(t.HasExpired);
        }
    }

    public class TimeBasedQueue<T>
    {
        readonly CommandQueue _commandQueue;
        readonly TimeSpan _durationToLastInQueue;
        readonly List<TimedEntry<T>> _eventsTracked = new List<TimedEntry<T>>();

        Action<IEnumerable<T>> _queryToApply;

        public TimeBasedQueue(CommandQueue queue, TimeSpan durationToLastInQueue, Action<IEnumerable<T>> queryToApply)
        {
            _commandQueue = queue;
            _durationToLastInQueue = durationToLastInQueue;
            _queryToApply = queryToApply;
        }

        public void Enqueue(T item)
        {
            //add item
            _commandQueue.Enqueue(() => _eventsTracked.Add(new TimedEntry<T>(item, _durationToLastInQueue)));

            //clean up list
            _commandQueue.Enqueue(PurgeListOfExpiredItems);

            //query list
            _commandQueue.Enqueue(() => _queryToApply(_eventsTracked.Select(x => x.Item)));
        }

        void PurgeListOfExpiredItems()
        {
            TimedEntry<T> i = _eventsTracked
                .First(x => !x.HasExpired);

            int index = _eventsTracked.IndexOf(i);

            _eventsTracked.RemoveRange(0, index + 1);
        }
    }

    public class TimedEntry<T>
    {
        readonly TimeSpan _duration;
        readonly DateTime _enteredAt = DateTime.Now;

        public TimedEntry(T item, TimeSpan duration)
        {
            Item = item;
            _duration = duration;
        }

        public T Item { get; private set; }

        public bool HasExpired
        {
            get
            {
                TimeSpan currentDuration = DateTime.Now.Subtract(_enteredAt);
                return currentDuration > _duration;
            }
        }

        public DateTime EnteredAt
        {
            get { return _enteredAt; }
        }
    }
}