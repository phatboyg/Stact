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
	using System.Threading;
	using DateTimeExtensions;
	using Magnum.Pipeline;
	using Magnum.Pipeline.Segments;
	using Messages;
	using NUnit.Framework;

	[TestFixture]
	public class Subscribing_with_an_interval
	{
		private Pipe _pipe;
		private ManualResetEvent _received;

		[SetUp]
		public void Setup()
		{
			_received = new ManualResetEvent(false);

			_pipe = PipeSegment.New();
		}

		[Test]
		public void Should_only_call_the_consumer_every_n_seconds()
		{
			int count = 0;
			using (var scope = _pipe.NewSubscriptionScope())
			{
				scope.Subscribe<ClaimModified>(1.Seconds(), messages =>
					{
						count = messages.Count;
						_received.Set();
					});

				_pipe.Send(new ClaimModified());
				_pipe.Send(new ClaimModified());
			}

			Assert.IsTrue(_received.WaitOne(2.Seconds(), true));
			Assert.AreEqual(2, count);
		}

		[Test]
		public void Should_only_include_the_messages_from_the_current_interval()
		{
			using (var scope = _pipe.NewSubscriptionScope())
			{
				int count = 0;
				scope.Subscribe<ClaimModified>(1.Seconds(), messages =>
				{
					count = messages.Count;
					_received.Set();
				});

				_pipe.Send(new ClaimModified());
				_pipe.Send(new ClaimModified());

				Assert.IsTrue(_received.WaitOne(2.Seconds(), true));
				Assert.AreEqual(2, count);

				_received.Reset();
				_pipe.Send(new ClaimModified());

				Assert.IsTrue(_received.WaitOne(2.Seconds(), true));
				Assert.AreEqual(1, count);
			}
		}
	}
}