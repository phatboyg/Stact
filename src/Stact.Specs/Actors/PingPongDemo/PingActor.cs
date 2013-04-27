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
namespace Stact.Specs.Actors.PingPongDemo
{
	using System.Diagnostics;
	using System.Threading;
	
	using Internal;


	public class PingActor :
		Ping
	{
		private int _initialCount;
		private int _pingCount;
		protected Fiber _fiber;
		private Stopwatch _watch;

		public PingActor()
		{
            _fiber = new TaskFiber();
		}

		public void Pong(Pong pong)
		{
			_fiber.Execute(() => Consume(pong));
		}

		public void Start(int count, Pong pong)
		{
			_initialCount = _pingCount = count;
			_watch = Stopwatch.StartNew();

			pong.Ping(this);
		}

		private void Consume(Pong pong)
		{
			_pingCount--;
			if (_pingCount == 0)
			{
				_watch.Stop();
				Trace.WriteLine("Ping -> Pong " + _pingCount + " On Thread: " + Thread.CurrentThread.ManagedThreadId);

				long perSecond = ((long) _initialCount*2000)/(_watch.ElapsedMilliseconds == 0 ? 1 : _watch.ElapsedMilliseconds);
				Trace.WriteLine("Elapsed Time = " + _watch.ElapsedMilliseconds + "ms, Messages Per Second = " + perSecond);
				return;
			}

			if (_pingCount%50000 == 0)
			{
				Trace.WriteLine("Ping -> Pong " + _pingCount + " On Thread: " + Thread.CurrentThread.ManagedThreadId);
			}

			pong.Ping(this);
		}
	}
}