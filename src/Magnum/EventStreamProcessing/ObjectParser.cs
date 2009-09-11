using System.Collections.Generic;

namespace Magnum.EventStreamProcessing
{
    public interface ObjectParser<TInputStream, TOutputValue>
    {
        IEnumerable<TOutputValue> Parse(TInputStream inFeed);
    }
}