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
	using System.IO;
	using System.Reflection;
	using Channels;
	using Fibers;
	using NUnit.Framework;
	using Visualizers;

	[TestFixture]
	public class GraphChannel_Specs
	{
		[Test]
		public void Should_contain_all_nodes()
		{
			var channel = new ChannelAdapter();
			channel.Connect(x =>
			{ 
				x.AddConsumerOf<SomeEvent>().UsingConsumer(m => { });
				x.AddConsumerOf<AnyEvent>().UsingConsumer(m => { });
				x.AddConsumerOf<AnyEvent>().UsingConsumer(m => { });
				x.AddConsumerOf<SomeEvent>().UsingInstanceOf<MyConsumer>(c => c.Input);
			});

			var generator = new ChannelGraphGenerator();

			string filename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "graph.png");

			generator.SaveGraphToFile(channel, 2560, 1920, filename);
		}

		private class MyConsumer
		{
			public MyConsumer()
			{
				Input = new ConsumerChannel<SomeEvent>(new SynchronousFiber(), m => { });
			}

			public Channel<SomeEvent> Input { get; private set; }
		}

		public interface AnyEvent
		{
		}

		public class SomeEvent :
			AnyEvent
		{
		}
	}
}