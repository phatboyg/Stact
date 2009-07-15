namespace FunctionalBits.Pipeline
{
    using System;
    using System.Collections.Generic;

    public abstract class PipelineNode :
        IPipeline
    {
        protected PipelineNode(PipelineType pipelineType, Type messageType)
        {
            NodeType = pipelineType;
            MessageType = messageType;
        }

        public virtual void Raise<T>(T message)
        {
            Accept(message).Each(consumer => consumer(message));
        }

        public virtual IEnumerable<MessageConsumer<T>> Accept<T>(T message)
        {
            yield break;
        }

        /// <summary>
        /// The type of this pipeline node
        /// </summary>
        public PipelineType NodeType { get; private set; }

        /// <summary>
        /// The type of data handled by the node
        /// </summary>
        public Type MessageType { get; private set; }

        public static Input Input(IPipeline pipeline)
        {
            return new Input(pipeline);
        }

        public static End End<T>()
        {
            return End(typeof (T));
        }

        public static End End(Type type)
        {
            return new End(type);
        }

        public static Filter Filter<TInput>(IPipeline pipeline)
        {
            return Filter(pipeline, typeof (TInput));
        }

        public static Filter Filter(IPipeline pipeline, Type inputType)
        {
            return new Filter(pipeline, inputType);
        }

        public static RecipientList RecipientList<T>(IEnumerable<IPipeline> recipients)
        {
            return RecipientList(typeof (T), recipients);
        }

        public static RecipientList RecipientList(Type messageType, IEnumerable<IPipeline> recipients)
        {
            return new RecipientList(messageType, recipients);
        }
    }
}