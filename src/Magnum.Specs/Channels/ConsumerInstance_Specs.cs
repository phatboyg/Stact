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
	using Magnum.Channels;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class Sending_a_message_to_an_instance_consumer
	{
		[Test]
		public void Should_create_a_new_consumer_instance_for_each_message()
		{
			var message = new MyMessage();

			var result = MockRepository.GenerateMock<Channel<MyMessage>>();
			result.Expect(x => x.Send(message)).Repeat.Twice();

			var provider = MockRepository.GenerateMock<ChannelProvider<MyMessage>>();
			provider.Expect(x => x(message)).Return(result).Repeat.Twice();

			var channel = new InstanceChannel<MyMessage>(provider);

			channel.Send(message);
			channel.Send(message);

			result.VerifyAllExpectations();
			provider.VerifyAllExpectations();
		}

		[Test]
		public void Should_pull_the_matching_instance_from_the_cace()
		{
			var message = new MyMessage();

			var result = MockRepository.GenerateMock<Channel<MyMessage>>();
			result.Expect(x => x.Send(message)).Repeat.Twice();

			var provider = MockRepository.GenerateMock<ChannelProvider<MyMessage>>();
			provider.Expect(x => x(message)).Return(result).Repeat.Once();

			KeySelector<MyMessage, Guid> messageKeySelector = x => x.Id;

			ChannelCache<MyMessage> cache = new DictionaryChannelCache<MyMessage, Guid>(provider, messageKeySelector);

			Channel<MyMessage> channel = cache.Get(message);
			channel.Send(message);

			channel = cache.Get(message);
			channel.Send(message);

			provider.VerifyAllExpectations();
			result.VerifyAllExpectations();
		}

		[Test]
		public void Should_work_for_primitive_types_shorty()
		{
			int message = 27;

			var result = MockRepository.GenerateMock<Channel<int>>();
			result.Expect(x => x.Send(message)).Repeat.Twice();

			var provider = MockRepository.GenerateMock<ChannelProvider<int>>();
			provider.Expect(x => x(message)).Return(result).Repeat.Once();

			KeySelector<int, int> messageKeySelector = x => x;

			ChannelCache<int> cache = new DictionaryChannelCache<int, int>(provider, messageKeySelector);

			Channel<int> channel = cache.Get(message);
			channel.Send(message);

			channel = cache.Get(message);
			channel.Send(message);

			provider.VerifyAllExpectations();
			result.VerifyAllExpectations();
		}
	}
}