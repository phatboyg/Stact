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
namespace Magnum.RulesEngine.Specs.Graphing
{
	using System;
	using System.IO;
	using System.Reflection;
	using Channels;
	using Channels.Visitors;
	using Extensions;
	using Fibers;
	using NUnit.Framework;
	using TestFramework;
	using Visualizers.Channels;


	[Scenario]
	public class Generating_a_graph_of_a_channel_network
	{
		[Given]
		public void Setup()
		{
			_channel = new ChannelAdapter();
			_channelConnection = _channel.Connect(x =>
				{
					x.AddConsumerOf<SomeEvent>()
						.UsingConsumer(m => { })
						.ExecuteOnProducerThread();

					x.AddConsumerOf<AnyEvent>()
						.UsingConsumer(m => { });

					x.AddConsumerOf<AnyEvent>()
						.UsingConsumer(m => { });

					x.AddConsumerOf<SomeEvent>()
						.UsingInstance()
						.Of<MyConsumer>()
						.ObtainedBy(m => new MyConsumer())
						.OnChannel(c => c.Input);

					x.AddConsumerOf<AnyEvent>()
						.BufferFor(5.Minutes())
						.Distinct(m => m.Key)
						.UsingConsumer(ms => { })
						.ExecuteOnThreadPoolFiber();

					x.AddConsumerOf<AnyEvent>()
						.Where(m => m.Key > 100)
						.UsingConsumer(m => { })
						.ExecuteOnThread();

					x.AddConsumerOf<SomeEvent>()
						.BufferFor(5.Minutes())
						.Last()
						.UsingConsumer(m => { })
						.ExecuteOnThreadPoolFiber();
				});
		}

		[Finally]
		public void Finally()
		{
			_channelConnection.Dispose();
		}

		[Then]
		public void Should_create_a_file()
		{
			var generator = new ChannelGraphGenerator();

			string filename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "graph.png");

			generator.SaveGraphToFile(_channel, 2560, 1920, filename);
		}

		[Then, Explicit]
		public void Should_launch_correctly_in_the_debug_visualizer()
		{
			ChannelDebugVisualizer.TestShowVisualizer(_channel.GetGraphData());
		}

		ChannelAdapter _channel;
		ChannelConnection _channelConnection;


		class MyConsumer
		{
			public MyConsumer()
			{
				Input = new ConsumerChannel<SomeEvent>(new SynchronousFiber(), m => { });
			}

			public Channel<SomeEvent> Input { get; private set; }
		}


		public interface AnyEvent
		{
			int Key { get; }
		}


		public class SomeEvent :
			AnyEvent
		{
			public int Key{get;set;}
		}
	}
}