namespace FunctionalBits.Pipeline
{
    using System.Collections.Generic;

    public class Input :
        PipelineNode
    {
        public Input(IPipeline pipeline)
            : base(PipelineType.Input, pipeline.MessageType)
        {
            Output = pipeline;
        }

        public IPipeline Output { get; private set; }

        public override IEnumerable<MessageConsumer<T>> Accept<T>(T message)
        {
            return Output.Accept(message);
        }
    }
}