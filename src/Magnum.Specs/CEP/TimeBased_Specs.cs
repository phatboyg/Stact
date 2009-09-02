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
        public void One_NoExpire()
        {
            var o = new object();
            var i = 0;
            var mre = new ManualResetEvent(false);
            var cq = new ThreadPoolCommandQueue();
            var q = new TimeBasedQueue<object>(cq, 1.Seconds(), (x)=>
            {
                i++;
                mre.Set();
            });
            q.Enqueue(o);
            cq.Run();


            Assert.IsTrue(mre.WaitOne(3000));
            Assert.AreEqual(1, i);
        }

        [Test]
        public void Two_NoExpire()
        {
            var o = new object();
            var i = 0;
            var mre = new ManualResetEvent(false);
            var cq = new ThreadPoolCommandQueue();
            var q = new TimeBasedQueue<object>(cq, 100.Seconds(), x =>
            {
                i++;
                if(i==2)mre.Set();
            });

            q.Enqueue(o);
            q.Enqueue(o);
            cq.Run();


            Assert.IsTrue(mre.WaitOne(3000));
            Assert.AreEqual(2, i);
        }

        [Test, Ignore] //how to test
        public void Two_OneExpire()
        {
            var o = new object();
            var i = 0;
            var mre = new ManualResetEvent(false);
            var cq = new SynchronousCommandQueue();
            var q = new TimeBasedQueue<object>(cq, 10.Milliseconds(), x =>
            {
                i++;
                Thread.Sleep(25);
            });


            cq.Run();

            q.Enqueue(o);
            q.Enqueue(o);
            q.Enqueue(o);

            //Assert.IsTrue(mre.WaitOne(3000));
            Assert.AreEqual(2, i);
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