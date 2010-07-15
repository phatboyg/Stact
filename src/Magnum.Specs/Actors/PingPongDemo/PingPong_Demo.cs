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
namespace Magnum.Specs.Actors.PingPongDemo
{
	using System.Diagnostics;
	using System.Threading;
	using Fibers;
	using NUnit.Framework;

	[TestFixture]
	[Category("Slow")]
	public class PingPong_Demo
	{
		[SetUp]
		public void Setup()
		{
			_ping = new PingActor();
			_ping2 = new PingActor();
			_ping3 = new PingActor();
			_ping4 = new PingActor();
			_pong = new PongActor();
		}

		private PingActor _ping;
		private PingActor _ping2;
		private PingActor _ping3;
		private PingActor _ping4;
		private Pong _pong;

		[Test, Explicit, Category("Demo")]
		public void Demo()
		{
			_ping.Start(10, _pong);
			Thread.Sleep(1000);

			_ping.Start(400000, _pong);
			_ping2.Start(400000, _pong);
			_ping3.Start(400000, _pong);
			_ping4.Start(400000, _pong);

			Thread.Sleep(30000);
		}

		[Test, Explicit, Category("Demo")]
		public void Timer_Based_Demo()
		{
			Scheduler scheduler = new TimerScheduler(new SynchronousFiber());
			scheduler.Schedule(0, 1000, new SynchronousFiber(), () =>
				{
					Trace.WriteLine("Starting it up");
					_ping.Start(10, _pong);
				});

			Thread.Sleep(6000);
		}
	}
}