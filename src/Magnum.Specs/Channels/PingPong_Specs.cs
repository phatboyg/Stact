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
	using Magnum.Actions;
	using Magnum.Actors;
	using Magnum.Channels;
	using Magnum.Extensions;
	using NUnit.Framework;
	using TestFramework;

	[TestFixture]
	public class Sending_a_ping_to_a_ponger
	{
		private class Ping
		{
			public Channel<Pong> ResponseChannel { get; set; }
		}

		private class Pong
		{
		}

		private class Pinger
		{
			private readonly ThreadPoolActionQueue _queue;

			public Pinger()
			{
				_queue = new ThreadPoolActionQueue();

				Ponged = new Future<Pong>();

				PongChannel = new ConsumerChannel<Pong>(_queue, HandlePong);
			}

			public Future<Pong> Ponged { get; private set; }
			public Channel<Pong> PongChannel { get; private set; }

			public void Ping(Channel<Ping> channel)
			{
				channel.Send(new Ping
					{
						ResponseChannel = PongChannel
					});
			}

			private void HandlePong(Pong pong)
			{
				Ponged.Complete(pong);
			}
		}

		private class Ponger
		{
			private readonly ThreadPoolActionQueue _queue;

			public Ponger()
			{
				_queue = new ThreadPoolActionQueue();

				Pinged = new Future<Ping>();

				PongChannel = new ConsumerChannel<Ping>(_queue, HandlePing);
			}

			public Future<Ping> Pinged { get; private set; }
			public Channel<Ping> PongChannel { get; private set; }

			private void HandlePing(Ping ping)
			{
				Pinged.Complete(ping);

				ping.ResponseChannel.Send(new Pong());
			}
		}

		[Test]
		public void Should_return_a_pong_to_the_pinger()
		{
			var pinger = new Pinger();
			var ponger = new Ponger();

			pinger.Ping(ponger.PongChannel);

			ponger.Pinged.IsAvailable(1.Seconds()).ShouldBeTrue();
			pinger.Ponged.IsAvailable(1.Seconds()).ShouldBeTrue();
		}
	}
}