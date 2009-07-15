namespace FunctionalBits.Pipeline
{
    using System;
    using System.Collections.Generic;

    public abstract class PipelineVisitor
    {
        protected virtual IPipeline Visit(IPipeline pipeline)
        {
            if (pipeline == null)
                return pipeline;

            switch (pipeline.NodeType)
            {
                case PipelineType.End:
                    return VisitEnd((End) pipeline);

                case PipelineType.Filter:
                    return VisitFilter((Filter) pipeline);

                case PipelineType.Input:
                    return VisitInput((Input) pipeline);

                case PipelineType.RecipientList:
                    return VisitRecipientList((RecipientList) pipeline);

                default:
                    throw new ArgumentException("The pipeline node is not a known type: " + pipeline.NodeType,
                                                "pipeline");
            }
        }

        protected virtual End VisitEnd(End end)
        {
            if (end == null)
                return null;

            return end;
        }

        protected virtual Filter VisitFilter(Filter filter)
        {
            if (filter == null)
                return null;

            IPipeline output = Visit(filter.Output);
            if (output != filter.Output)
            {
                return new Filter(output, filter.MessageType);
            }

            return filter;
        }

        protected virtual Input VisitInput(Input input)
        {
            if (input == null)
                return null;

            IPipeline pipeline = Visit(input.Output);
            if (pipeline != input.Output)
            {
                return new Input(pipeline);
            }

            return input;
        }

        protected virtual RecipientList VisitRecipientList(RecipientList recipientList)
        {
            if (recipientList == null)
                return null;

            bool modified = false;
            IList<IPipeline> recipients = new List<IPipeline>();

            foreach (IPipeline recipient in recipientList.Recipients)
            {
                IPipeline result = Visit(recipient);
                if (result != recipient)
                {
                    modified = true;
                }

                if (result != null)
                    recipients.Add(result);
            }

            if (modified)
            {
                return new RecipientList(recipientList.MessageType, recipients);
            }

            return recipientList;
        }
    }
}