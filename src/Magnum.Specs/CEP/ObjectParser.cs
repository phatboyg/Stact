namespace Magnum.Specs.CEP
{
    using System.Collections.Generic;

    public interface ObjectParser<TInputStream, TOutputValue>
    {
        IEnumerable<TOutputValue> Parse(TInputStream inFeed);
    }
}