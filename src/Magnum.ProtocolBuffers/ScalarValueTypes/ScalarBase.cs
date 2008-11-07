namespace Magnum.ProtocolBuffers.ScalarValueTypes
{
    using System;

    public abstract class ScalarBase
    {
        public abstract string Name { get; }
        public abstract Type DotNetType { get; }
    }
}