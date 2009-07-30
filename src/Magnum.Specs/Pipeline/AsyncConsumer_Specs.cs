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
	using Consumers;
	using DateTimeExtensions;
	using Messages;
	using NUnit.Framework;

	[TestFixture]
	public class Sending_a_message_with_an_asynchronous_consumer :
		Given_an_established_pipe
	{
		private LongRunningMessageConsumer _consumer;

		protected override void EstablishContext()
		{
			base.EstablishContext();

			_consumer = new LongRunningMessageConsumer();

			Scope.Subscribe(_consumer);

			Input.Send(new ClaimModified());
		}

		[Test]
		public void Should_return_immediately_while_the_consumer_still_executes()
		{
			Assert.IsFalse(_consumer.ClaimModifiedCalled.IsAvailable());
		}

		[Test]
		public void Should_complete_eventually()
		{
			Assert.IsTrue(_consumer.ClaimModifiedCalled.IsAvailable(2.Seconds()));
		}
	}
}