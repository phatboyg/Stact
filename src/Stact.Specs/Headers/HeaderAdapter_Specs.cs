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
namespace Stact.Specs.Headers
{
	using Internal;
	using Magnum.TestFramework;
	using Model;
	using NUnit.Framework;


	[Scenario]
	public class When_converting_messages_between_header_types
	{
		Future<Simple> _received;

		[SetUp]
		public void Converting_messages_between_header_types()
		{
			_received = new Future<Simple>();
		}

		[Then]
		public void Should_pass_through_a_raw_message()
		{
			HeaderTypeAdapter<Simple>.TryConvert<Simple>(new SimpleImpl(), _received.Complete).ShouldBeTrue();
			_received.IsCompleted.ShouldBeTrue();
			_received.Value.ShouldNotBeNull();
		}

		[Then]
		public void Should_pass_an_implementation_to_an_interface_type()
		{
			HeaderTypeAdapter<Simple>.TryConvert(new SimpleImpl(), _received.Complete).ShouldBeTrue();
			_received.IsCompleted.ShouldBeTrue();
			_received.Value.ShouldNotBeNull();
		}

		[Then]
		public void Should_downconvert_a_message_of_type_to_a_raw_message()
		{
			HeaderTypeAdapter<Simple>.TryConvert(new MessageImpl<Simple>(new SimpleImpl()), _received.Complete).ShouldBeTrue();
			_received.IsCompleted.ShouldBeTrue();
			_received.Value.ShouldNotBeNull();
		}

		[Then]
		public void Should_downconvert_a_message_of_an_implementation_of_type_to_a_raw_message()
		{
			HeaderTypeAdapter<Simple>.TryConvert(new MessageImpl<SimpleImpl>(new SimpleImpl()), _received.Complete).ShouldBeTrue();
			_received.IsCompleted.ShouldBeTrue();
			_received.Value.ShouldNotBeNull();
		}

		[Then]
		public void Should_downconvert_a_response_message_of_type_to_a_raw_message()
		{
			HeaderTypeAdapter<Simple>.TryConvert(new ResponseImpl<Simple>(new SimpleImpl()), _received.Complete).ShouldBeTrue();
			_received.IsCompleted.ShouldBeTrue();
			_received.Value.ShouldNotBeNull();
		}

		[Then]
		public void Should_downconvert_a_request_message_of_type_to_a_request_message()
		{
			var received = new Future<Request<Simple>>();
			HeaderTypeAdapter<Request<Simple>>.TryConvert(new RequestImpl<Simple>(null, new SimpleImpl()), received.Complete).ShouldBeTrue();
			received.IsCompleted.ShouldBeTrue();
			received.Value.ShouldNotBeNull();

		}
	}

	[Scenario]
	public class When_converting_request_types
	{
		[Then]
		public void Should_downconvert_a_request_message_of_type_to_a_request_message()
		{
			var received = new Future<Request<Simple>>();
			HeaderTypeAdapter<Request<Simple>>.TryConvert<Request<SimpleImpl>>(new RequestImpl<SimpleImpl>(null, new SimpleImpl()), received.Complete).ShouldBeTrue();
			received.IsCompleted.ShouldBeTrue();
			received.Value.ShouldNotBeNull();
		}
	}

	[Scenario]
	public class When_converting_response_types
	{
		[Then]
		public void Should_downconvert_a_response_message_of_type_to_a_response_message()
		{
			var received = new Future<Response<Simple>>();
			HeaderTypeAdapter<Response<Simple>>.TryConvert<Response<SimpleImpl>>(new ResponseImpl<SimpleImpl>(new SimpleImpl()), received.Complete).ShouldBeTrue();
			received.IsCompleted.ShouldBeTrue();
			received.Value.ShouldNotBeNull();
		}

		[Then]
		public void Should_downconvert_a_response_message_of_type_to_a_response_message_with_a_request()
		{
			var received = new Future<Response<Simple>>();
			HeaderTypeAdapter<Response<Simple>>.TryConvert<Response<Ask, SimpleImpl>>(new ResponseImpl<Ask, SimpleImpl>(null, new SimpleImpl()), received.Complete).ShouldBeTrue();
			received.IsCompleted.ShouldBeTrue();
			received.Value.ShouldNotBeNull();
		}

		[Then]
		public void Should_work_through_a_channel_network()
		{
			var received = new FutureChannel<Request<Simple>>();

			UntypedChannel channel = new ChannelAdapter();
			channel.Connect(x => x.AddChannel(received));

			var simpleImpl = new SimpleImpl();	
			channel.Send(new RequestImpl<SimpleImpl>(null, simpleImpl));

			received.IsCompleted.ShouldBeTrue();
			received.Value.ShouldNotBeNull();
			received.Value.Body.ShouldEqual(simpleImpl);
		}
	}
}