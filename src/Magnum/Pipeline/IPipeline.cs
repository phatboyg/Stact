namespace FunctionalBits.Pipeline
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A pipeline is used for the delivery of messages (which are really just objects of any type)
    /// </summary>
    public interface IPipeline
    {
        /// <summary>
        /// Raises an event through the pipeline
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        void Raise<T>(T message);

        /// <summary>
        /// Returns an enumeration of consumers that are interested in the message
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        IEnumerable<MessageConsumer<T>> Accept<T>(T message);

        /// <summary>
        /// The type of this pipeline node
        /// </summary>
        PipelineType NodeType { get; }

        /// <summary>
        /// The type of data handled by the node
        /// </summary>
        Type MessageType { get; }
    }
}