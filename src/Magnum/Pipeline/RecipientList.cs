namespace FunctionalBits.Pipeline
{
    using System;
    using System.Collections.Generic;

    public class RecipientList :
        PipelineNode
    {
        public IEnumerable<IPipeline> Recipients { get; private set; }

        public RecipientList(Type messageType, IEnumerable<IPipeline> recipients)
            : base(PipelineType.RecipientList, messageType)
        {
            Recipients = new List<IPipeline>(recipients);
        }

        public override IEnumerable<MessageConsumer<T>> Accept<T>(T message)
        {
            if (!MessageType.IsAssignableFrom(typeof (T)))
                yield break;

            foreach (IPipeline pipeline in Recipients)
            {
                foreach (var consumer in pipeline.Accept(message))
                {
                    yield return consumer;
                }
            }
        }
    }
}