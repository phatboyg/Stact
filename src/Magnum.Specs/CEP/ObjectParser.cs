namespace Magnum.Specs.CEP
{
    using System.Collections.Generic;

    public interface ObjectParser
    {
        IEnumerable<PatternMatches> Parse(IEnumerable<object> inFeed);
    }

    public interface PatternMatches
    {
        
    }
}