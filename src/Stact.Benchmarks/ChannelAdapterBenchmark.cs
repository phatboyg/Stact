// Copyright 2010 Chris Patterson
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
namespace Stact.Benchmarks
{
	using System;
	using System.Diagnostics;


//	public class ChannelAdapterBenchmark
//	{
//		public void Run()
//		{
//			Stopwatch timer = Stopwatch.StartNew();
//
//			const int channelCount = 10000;
//			const int seedCount = 500;
//
//			var channels = new UntypedChannel[channelCount];
//			var connections = new ChannelConnection[channelCount];
//
//			var complete = new Future<int>();
//
//			var latch = new CountdownLatch(channelCount*seedCount, complete.Complete);
//
//			for (int i = 0; i < channelCount; i++)
//			{
//				int channelNumber = i;
//				channels[i] = new ChannelAdapter();
//				connections[i] = channels[i].Connect(x =>
//					{
//						x.AddConsumerOf<AMessage>()
//							.UsingConsumer(message =>
//								{
//									if (channelNumber < channels.Length - 1)
//										channels[channelNumber + 1].Send(message);
//
//									latch.CountDown();
//								});
//					});
//			}
//
//			var body = new AMessage();
//
//			for (int i = 0; i < seedCount; i++)
//			{
//				channels[i].Send(body);
//
//				for (int j = 0; j < i; j++)
//					latch.CountDown();
//			}
//
//			bool completed = complete.WaitUntilCompleted(2.Minutes());
//
//			timer.Stop();
//
//			connections.Each(x => x.Dispose());
//			
//
//			if (!completed)
//			{
//				Console.WriteLine("Process did not complete");
//				return;
//			}
//
//			Console.WriteLine("Processed {0} messages in with {1} channels in {2}ms", seedCount, channelCount,
//			                  timer.ElapsedMilliseconds);
//
//			Console.WriteLine("That's {0} messages per second!", ((long)seedCount*channelCount*1000)/timer.ElapsedMilliseconds);
//		}
//
//
//		class AMessage
//		{
//		}
//	}
}