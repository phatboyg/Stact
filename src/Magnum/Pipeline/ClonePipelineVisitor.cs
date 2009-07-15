namespace FunctionalBits.Pipeline
{
    public class ClonePipelineVisitor :
        PipelineVisitor
    {
        public IPipeline Clone(IPipeline pipeline)
        {
            return base.Visit(pipeline);
        }
    }
}