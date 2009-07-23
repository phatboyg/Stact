// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace Magnum.Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Segments;
    using Visitors;

    public class RecipientListBinder :
        AbstractPipeVisitor
    {
        private readonly Type _messageType;
        private readonly Pipe _segment;
        private bool _found;

        public RecipientListBinder(Pipe segment)
        {
            _messageType = segment.MessageType;
            _segment = segment;
        }

        public void Bind(Pipe pipe)
        {
            base.Visit(pipe);

            if (_found == false)
            {
                throw new PipelineException("Could not bind pipe: " + pipe.SegmentType + "(" + pipe.MessageType + ")");
            }
        }

        protected override Pipe VisitInput(InputSegment input)
        {
            Pipe pipe = base.VisitInput(input);

            if (pipe != input)
                throw new InvalidOperationException("The input should never change");

            if (!_found && input.MessageType == typeof (object))
            {
                var recipients = input.Output.SegmentType == PipeSegmentType.End ? new Pipe[] {} : new[] {input.Output};
                Pipe list = PipeSegment.RecipientList(typeof (object), recipients);

                Pipe result = Visit(list);

                input.ReplaceOutput(input.Output, result);
            }

            return input;
        }

        protected override Pipe VisitRecipientList(RecipientListSegment recipientList)
        {
            if (recipientList == null)
                return null;

            if (recipientList.MessageType != _messageType)
            {
                if (recipientList.MessageType == typeof (object))
                    return VisitObjectRecipientList(recipientList);

                return base.VisitRecipientList(recipientList);
            }

            IList<Pipe> recipients = VisitRecipients(recipientList.Recipients);

            recipients.Add(_segment);
            _found = true;

            return new RecipientListSegment(recipientList.MessageType, recipients);
        }

        private Pipe VisitObjectRecipientList(RecipientListSegment recipientList)
        {
            var result = base.VisitRecipientList(recipientList) as RecipientListSegment;
            if (result == null)
                return null;

            if (_found)
                return result;

            Pipe list = PipeSegment.RecipientList(_segment.MessageType, new[] {_segment});
            Pipe filter = PipeSegment.Filter(list, _segment.MessageType);

            IList<Pipe> recipients = new List<Pipe>(result.Recipients);
            recipients.Add(filter);

            _found = true;

            return new RecipientListSegment(recipientList.MessageType, recipients);
        }

        private IList<Pipe> VisitRecipients(IEnumerable<Pipe> recipients)
        {
            IList<Pipe> newRecipients = new List<Pipe>();

            foreach (Pipe recipient in recipients)
            {
                Pipe result = Visit(recipient);
                if (result != null)
                    newRecipients.Add(result);

                if (result == _segment)
                    throw new InvalidOperationException("A segment is attempting to subscribe again!");
            }

            return newRecipients;
        }
    }
}