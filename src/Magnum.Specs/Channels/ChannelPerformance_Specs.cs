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
namespace Magnum.Specs.Channels
{
	using System.Diagnostics;
	using Magnum.Actions;
	using Magnum.Channels;
	using Magnum.Extensions;
	using NUnit.Framework;
	using TestFramework;

	[TestFixture]
	public class The_performance_of_the_channel_api
	{
		private struct MsgStruct
		{
			public int Count;
			public long Other;
		}

		[Test, Explicit]
		public void Should_be_fast()
		{
			ActionQueue queue = new ThreadPoolActionQueue();

			const int limit = 5000000;

			var complete = new Future<int>();

			Channel<MsgStruct> channel = new ConsumerChannel<MsgStruct>(queue, message =>
				{
					if (message.Count == limit)
						complete.Complete(limit);
				});

			using (var timer = new FunctionTimer("Throughput", x =>
				{
					Trace.WriteLine("Time to execute: " + (int) x.ElapsedMilliseconds + "ms");

					Trace.WriteLine("Per second throughput: " + (int) (limit/(x.ElapsedMilliseconds/1000)));
				}))
			{
				for (int i = 1; i <= limit; i++)
				{
					channel.Send(new MsgStruct
						{
							Count = i,
							Other = i*8,
						});
				}

				timer.Mark();

				complete.WaitUntilCompleted(30.Seconds()).ShouldBeTrue();
			}
		}
	}
}