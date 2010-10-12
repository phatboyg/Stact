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
	using NUnit.Framework;


	[Scenario]
	public class When_converting_messages_between_header_types
	{
		HeaderTypeAdapter<Simple> _adapter;
		Future<Simple> _received;

		[SetUp]
		public void Converting_messages_between_header_types()
		{
			_received = new Future<Simple>();
			_adapter = new HeaderTypeAdapter<Simple>();
		}

		[Then]
		public void Should_pass_through_a_raw_message()
		{
			_adapter.TryConvert<Simple>(new SimpleImpl(), _received.Complete).ShouldBeTrue();
			_received.IsCompleted.ShouldBeTrue();
		}

		[Then]
		public void Should_pass_an_implementation_to_an_interface_type()
		{
			_adapter.TryConvert(new SimpleImpl(), _received.Complete).ShouldBeTrue();
			_received.IsCompleted.ShouldBeTrue();
		}

		[Then]
		public void Should_downconvert_a_message_of_type_to_a_raw_message()
		{
			_adapter.TryConvert(new MessageImpl<Simple>(new SimpleImpl()), _received.Complete).ShouldBeTrue();
			_received.IsCompleted.ShouldBeTrue();
		}

		[Then]
		public void Should_downconvert_a_message_of_an_implementation_of_type_to_a_raw_message()
		{
			_adapter.TryConvert(new MessageImpl<SimpleImpl>(new SimpleImpl()), _received.Complete).ShouldBeTrue();
			_received.IsCompleted.ShouldBeTrue();
		}

		[Then]
		public void Should_downconvert_a_response_message_of_type_to_a_raw_message()
		{
			_adapter.TryConvert(new ResponseImpl<Simple>(new SimpleImpl()), _received.Complete).ShouldBeTrue();
			_received.IsCompleted.ShouldBeTrue();
		}
	}
}