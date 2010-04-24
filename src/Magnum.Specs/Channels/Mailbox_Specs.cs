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
	using System;
	using Fibers;
	using Magnum.Channels;
	using Magnum.Extensions;
	using NUnit.Framework;
	using TestFramework;

	[TestFixture]
	public class Receiving_a_message_from_a_mailbox
	{
		[SetUp]
		public void Setup()
		{
			_mailbox = new DefaultMailbox<TestMessage>(new SynchronousFiber(), new TimerScheduler(new SynchronousFiber()));
			_transactionId = CombGuid.Generate();
			_received = new Future<TestMessage>();
		}

		private DefaultMailbox<TestMessage> _mailbox;
		private Guid _transactionId;
		private Future<TestMessage> _received;

		private class TestMessage
		{
			public Guid Id { get; set; }
		}

		[Test]
		public void Should_deliver_a_new_message_to_a_waiting_receiver()
		{
			_mailbox.Receive(message =>
				{
					if (message.Id != _transactionId)
						return null;

					return msg => _received.Complete(msg);
				});

			_mailbox.Send(new TestMessage
				{
					Id = _transactionId,
				});

			_received.WaitUntilCompleted(2.Seconds()).ShouldBeTrue();
		}

		[Test]
		public void Should_not_deliver_an_unwanted_message_to_a_receiver()
		{
			_mailbox.Receive(message =>
				{
					if (message.Id != _transactionId)
						return null;

					return msg => _received.Complete(msg);
				});

			_mailbox.Send(new TestMessage
				{
					Id = CombGuid.Generate(),
				});

			_received.WaitUntilCompleted(2.Seconds()).ShouldBeFalse();

			_mailbox.Receive(message => msg => _received.Complete(msg));

			_received.WaitUntilCompleted(2.Seconds()).ShouldBeTrue();
		}

		[Test]
		public void Should_deliver_an_old_message_to_a_new_receiver()
		{
			_mailbox.Send(new TestMessage
			{
				Id = _transactionId,
			});

			_mailbox.Receive(message =>
				{
					if (message.Id != _transactionId)
						return null;

					return msg => _received.Complete(msg);
				});

			_received.WaitUntilCompleted(2.Seconds()).ShouldBeTrue();
		}
	}
}