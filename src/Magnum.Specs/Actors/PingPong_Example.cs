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
namespace Magnum.Specs.Actors
{
	using System.Diagnostics;
	using System.Threading;
	using Magnum.Actors;
	using Magnum.Actors.CommandQueues;
	using MbUnit.Framework;

	[TestFixture]
	public class PingPong_Example
	{
		private IPingPongPlayer _jack;
		private IPingPongPlayer _jill;

		[SetUp]
		public void Setup()
		{
			_jack = new PingPongPlayer("Jack").GetProxy();
			_jill = new PingPongPlayer("Jill").GetProxy();
		}

		[Test, Explicit]
		public void Show_the_ping_pong_blowing_the_stack()
		{
			_jack.PingPong(1000000, _jill);

			Thread.Sleep(10000);
		}

		[Test]
		public void Show_the_ping_pong_going_back_and_forth()
		{
			_jack.PingPong(1000, _jill);

			Thread.Sleep(10000);
		}
	}

	public interface IPingPongPlayer
	{
		void PingPong(int count, IPingPongPlayer partner);
	}

	public class PingPongPlayer : IPingPongPlayer
	{
		private readonly string _name;
		private IPingPongPlayer _actorProxy;
		private CommandQueue _queue = new ThreadPoolCommandQueue();
		private Stopwatch _watch = Stopwatch.StartNew();

		public PingPongPlayer(string name)
		{
			_name = name;

			_queue.Run();
			_actorProxy = new PingPongPlayerProxy(this);
		}

		public void PingPong(int count, IPingPongPlayer partner)
		{
			if (count == 0)
			{
				_watch.Stop();
				Trace.WriteLine(_name + ": done in " + _watch.ElapsedMilliseconds + "ms");

				return;
			}

			if (count%5000 == 0 || count%5000 == 1)
			{
				//Trace.WriteLine(_name + ": PingPong = " + pingPongParams.Count + " On Thread: " + Thread.CurrentThread.ManagedThreadId);
			}

			partner.PingPong(count - 1, _actorProxy);
		}

		public IPingPongPlayer GetProxy()
		{
			return _actorProxy;
		}

		public class PingPongPlayerProxy : IPingPongPlayer
		{
			private readonly PingPongPlayer _player;

			public PingPongPlayerProxy(PingPongPlayer player)
			{
				_player = player;
			}

			public void PingPong(int count, IPingPongPlayer partner)
			{
				_player._queue.Enqueue(() => _player.PingPong(count, partner));
			}
		}
	}
}