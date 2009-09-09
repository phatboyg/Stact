namespace Magnum.Specs.CEP
{
    public class Result<TUnconsummedInput, TValue>
    {
        public Result(TValue value, TUnconsummedInput rest)
        {
            Value = value;
            Rest = rest;
        }

        public TValue Value { get; private set; }
        public TUnconsummedInput Rest { get; private set; }
    }
}