namespace Magnum.Common.Metrics
{
    public class Counter
    {
        private int _count;
        public string Description { get; private set; }

        public Counter() : this("unnamed")
        {
        }

        public Counter(string description)
        {
            Description = description;
        }

        public void Increment()
        {
            _count++;
        }

        public void Decrement()
        {
            _count--;
        }

        public int CurrentValue()
        {
            return _count;
        }

        public void Reset()
        {
            _count = 0;
        }

    }
}