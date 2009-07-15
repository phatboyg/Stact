namespace FunctionalBits.Pipeline
{
    using System;

    public class TracePipelineVisitor :
        PipelineVisitor
    {
        private int _depth;

        public IPipeline Trace(IPipeline pipeline)
        {
            return base.Visit(pipeline);
        }

        protected override Input VisitInput(Input input)
        {
            WriteLine(input);

            return base.VisitInput(input);
        }

        protected override End VisitEnd(End end)
        {
            WriteLine(end);

            return base.VisitEnd(end);
        }

        protected override Filter VisitFilter(Filter filter)
        {
            WriteLine(filter, "Allow " + filter.Output.MessageType);

            return base.VisitFilter(filter);
        }

        protected override RecipientList VisitRecipientList(RecipientList recipientList)
        {
            WriteLine(recipientList);
            Indent(() =>
                       {
                           recipientList.Recipients.Each(recipient => Visit(recipient));
                       });

            return base.VisitRecipientList(recipientList);
        }

        private void WriteLine(IPipeline node)
        {
            WriteLine(node, "");
        }

        private void WriteLine(IPipeline node, string message)
        {
            string formatted = string.Format(Pad() + "{0}<{1}>: {2}", node.NodeType, node.MessageType.Name, message);

            System.Diagnostics.Trace.WriteLine(formatted);
        }

        private string Pad()
        {
            return new string('\t', _depth);
        }

        private void Indent(Action action)
        {
            _depth++;
            try
            {
                action();
            }
            finally
            {
                _depth--;
            }
        }

        private T Indent<T>(Func<T> visitor)
        {
            _depth++;
            try
            {
                return visitor();
            }
            finally
            {
                _depth--;
            }
        }
    }
}