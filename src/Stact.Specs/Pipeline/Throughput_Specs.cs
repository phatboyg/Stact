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
namespace Stact.Specs.Pipeline
{
	using System.Diagnostics;
	using System.Threading;
	using Stact.Pipeline;
	using Stact.Pipeline.Segments;
	using Messages;
	using NUnit.Framework;

	[TestFixture]
	public class Throughput_Specs
	{
		private long _count;
		private long _count2;
		private long _limit;
		private Pipe _input;

		[Test, Explicit]
		public void How_many_messages_can_the_pipe_send_per_second()
		{
			_count = 0;
			_count2 = 0;
			_limit = 5000000;

			Pipe consumer = PipeSegment.Consumer<ClaimModified>(m => { Interlocked.Increment(ref _count); });
			Pipe consumer2 = PipeSegment.Consumer<ClaimModified>(m => { Interlocked.Increment(ref _count2); });

			var recipients = new[] {consumer, consumer2};

			Pipe recipientList = PipeSegment.RecipientList<ClaimModified>(recipients);
			Pipe filter = PipeSegment.Filter<object>(recipientList);
			Pipe objectRecipientList = PipeSegment.RecipientList<object>(new[] {filter});
			_input = PipeSegment.Input(objectRecipientList);

			var message = new ClaimModified();

			for (int i = 0; i < 100; i++)
			{
				_input.Send(message);
			}

			_count = 0;
			_count2 = 0;

			Thread pusherThread = new Thread(Pusher);
			Thread pusherThread2 = new Thread(Pusher);

			Stopwatch timer = Stopwatch.StartNew();

			pusherThread.Start();
			pusherThread2.Start();

			pusherThread.Join(10000);
			pusherThread2.Join(1000);

			timer.Stop();

			Trace.WriteLine("Received: " + (_count + _count2) + ", expected " + _limit*2);
			Trace.WriteLine("Elapsed Time: " + timer.ElapsedMilliseconds + "ms");
			Trace.WriteLine("Messages Per Second: " + _limit*1000/timer.ElapsedMilliseconds);
		}

		private void Pusher()
		{
			var message = new ClaimModified();
			for (int i = 0; i < _limit/2; i++)
			{
				_input.Send(message);
			}
		}
	}
}