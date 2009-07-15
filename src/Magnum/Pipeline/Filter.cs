namespace FunctionalBits.Pipeline
{
    using System;
    using System.Collections.Generic;

    public class Filter :
        PipelineNode
    {
        public Filter(IPipeline pipeline, Type inputType)
            : base(PipelineType.Filter, inputType)
        {
            Output = pipeline;
        }

        public IPipeline Output { get; private set; }

        public override IEnumerable<MessageConsumer<T>> Accept<T>(T message)
        {
            if (Output.MessageType.IsAssignableFrom(typeof (T)))
            {
                foreach (var consumer in Output.Accept(message))
                {
                    yield return consumer;
                }
            }
        }
    }
}