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
namespace Stact.Specs.Channels
{
	using System;
	
	using Internal;
	using Magnum;
	using Stact.Actors.Internal;
	using Stact.Channels;
	using Magnum.Extensions;
	using NUnit.Framework;
	using Magnum.TestFramework;

	[TestFixture]
	public class Receiving_a_message_from_an_inbox
	{
		[SetUp]
		public void Setup()
		{
			_inbox = new BufferedInbox<TestMessage>(new PoolFiber(), new TimerScheduler(new PoolFiber()));
			_transactionId = CombGuid.Generate();
			_received = new Stact.Future<TestMessage>();
		}

		private BufferedInbox<TestMessage> _inbox;
		private Guid _transactionId;
		private Stact.Future<TestMessage> _received;

		private class TestMessage
		{
			public Guid Id { get; set; }
		}

		[Test]
		public void Should_deliver_a_new_message_to_a_waiting_receiver()
		{
			_inbox.Receive(message =>
				{
					if (message.Id != _transactionId)
						return null;

					return msg => _received.Complete(msg);
				});

			_inbox.Send(new TestMessage
				{
					Id = _transactionId,
				});

			_received.WaitUntilCompleted(2.Seconds()).ShouldBeTrue();
		}

		[Test]
		public void Should_not_deliver_an_unwanted_message_to_a_receiver()
		{
			_inbox.Receive(message =>
				{
					if (message.Id != _transactionId)
						return null;

					return msg => _received.Complete(msg);
				});

			_inbox.Send(new TestMessage
				{
					Id = CombGuid.Generate(),
				});

			_received.WaitUntilCompleted(2.Seconds()).ShouldBeFalse();

			_inbox.Receive(message => msg => _received.Complete(msg));

			_received.WaitUntilCompleted(2.Seconds()).ShouldBeTrue();
		}

		[Test]
		public void Should_deliver_an_old_message_to_a_new_receiver()
		{
			_inbox.Send(new TestMessage
			{
				Id = _transactionId,
			});

			_inbox.Receive(message =>
				{
					if (message.Id != _transactionId)
						return null;

					return msg => _received.Complete(msg);
				});

			_received.WaitUntilCompleted(2.Seconds()).ShouldBeTrue();
		}
	}
}