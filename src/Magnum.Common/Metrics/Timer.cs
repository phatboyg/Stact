namespace Magnum.Common.Metrics
{
    using System;
    using System.Collections.Generic;

    public class Timer
    {
        private readonly IList<TimeSpan> _capturedRuns = new List<TimeSpan>();
        public string Description { get; private set; }

        public Timer(string description)
        {
            Description = description;
        }

        public Timer() : this("unnamed")
        {
        }

        public TimeSpan AverageRunTime
        {
            get
            {
                return new TimeSpan(0);
            }
        }

        public void Reset()
        {
            _capturedRuns.Clear();
        }

        public TimerDispose Start()
        {
            return new TimerDispose();
        }

        public class TimerDispose :
            IDisposable
        {
            public void Dispose()
            {
                //stop timer
            }
        }
    }
}