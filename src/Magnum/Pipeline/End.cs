namespace FunctionalBits.Pipeline
{
    using System;

    public class End :
        PipelineNode
    {
        public End(Type type)
            : base(PipelineType.End, type)
        {
        }
    }
}