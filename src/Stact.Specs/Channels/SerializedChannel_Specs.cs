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
	using Magnum.Serialization;
	using Stact.Channels;
	using NUnit.Framework;
	using Magnum.TestFramework;

	[TestFixture]
	public class Sending_a_message_through_a_serialized_channel
	{
		[Test]
		public void Should_result_in_the_proper_message_data_arriving()
		{
			var message = new TestMessage {Id = CombGuid.Generate(), Name = "Chris"};

			var future = new Stact.Future<TestMessage>();

			var consumerChannel = new ConsumerChannel<TestMessage>(new SynchronousFiber(), future.Complete);
			var deserializeChannel = new DeserializeChannel<TestMessage>(new SynchronousFiber(), new FastTextSerializer(),
			                                                             consumerChannel);
			var serializeChannel = new SerializeChannel<TestMessage>(new SynchronousFiber(), new FastTextSerializer(),
			                                                         deserializeChannel);

			serializeChannel.Send(message);

			future.IsCompleted.ShouldBeTrue();
			future.Value.ShouldNotBeTheSameAs(message);
			future.Value.ShouldEqual(message);
		}


		private class TestMessage
		{
			public Guid Id { get; set; }
			public string Name { get; set; }

			public bool Equals(TestMessage other)
			{
				if (ReferenceEquals(null, other)) return false;
				if (ReferenceEquals(this, other)) return true;
				return other.Id.Equals(Id) && Equals(other.Name, Name);
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj.GetType() != typeof (TestMessage)) return false;
				return Equals((TestMessage) obj);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					return (Id.GetHashCode()*397) ^ (Name != null ? Name.GetHashCode() : 0);
				}
			}
		}
	}
}