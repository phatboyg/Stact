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
namespace Magnum.Specs.Pipeline
{
    using System.Diagnostics;
    using Magnum.Pipeline;
    using Magnum.Pipeline.Segments;
    using NUnit.Framework;

    [TestFixture]
    public class Throughput_Specs
    {
        [Test, Explicit]
        public void How_many_messages_can_the_pipe_send_per_second()
        {
            long count = 0;
            long limit = 2500000;

            Pipe consumer = PipeSegment.Consumer<ClaimModified>(m => { count++; });

            var recipients = new[] {consumer};

            Pipe recipientList = PipeSegment.RecipientList<ClaimModified>(recipients);
            Pipe filter = PipeSegment.Filter<object>(recipientList);
            Pipe input = PipeSegment.Input(filter);

            var message = new ClaimModified();

            Stopwatch timer = Stopwatch.StartNew();

            for (int i = 0; i < limit; i++)
            {
                input.Send(message);
            }

            timer.Stop();

            Trace.WriteLine("Elapsed Time: " + timer.ElapsedMilliseconds + "ms");
            Trace.WriteLine("Messages Per Second: " + limit*1000/timer.ElapsedMilliseconds);
        }
    }
}