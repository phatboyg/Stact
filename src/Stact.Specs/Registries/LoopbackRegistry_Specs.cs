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
namespace Stact.Specs.Registries
{
	using System;
	using Magnum.Extensions;
	using Magnum.TestFramework;


	[Scenario]
	public class Given_a_loopback_registry
	{
		protected ActorInstance Actor { get; private set; }
		protected Guid ActorId { get; private set; }
		protected Uri Address { get; private set; }
		protected Future<A> ReceivedA { get; private set; }
		protected ActorRegistry Registry { get; private set; }

		[Given]
		public void A_loopback_registry()
		{
			Address = new Uri("loopback://localhost/");
			ReceivedA = new Future<A>();

			Registry = ActorRegistryFactory.New(x =>
			{
				x.Remote(r =>
				{
					r.ListenTo(Address);
				});
			});

			ActorId = Guid.NewGuid();
			Actor = AnonymousActor.New(inbox =>
			{
				inbox.Loop(loop =>
				{
					loop.Receive<Message<A>>(message =>
					{
						ReceivedA.Complete(message.Body);

						loop.Repeat();
					});
				});
			});

			Registry.Register(ActorId, Actor);
		}


		protected class A
		{
			public string Name { get; set; }
		}


		public class B
		{
			public bool Valid { get; set; }
		}
	}


	[Scenario]
	public class When_a_message_is_sent_to_the_registry_directly :
		Given_a_loopback_registry
	{
		[Then]
		public void Should_resolve_a_destination_address()
		{
			Registry.Send(new A
			{
				Name = "bob"
			}, msg =>
			{
				msg.DestinationAddress = new ActorUrn(ActorId);
			});

			ReceivedA.WaitUntilCompleted(2.Seconds()).ShouldBeTrue();
		}
	}


	[Scenario]
	public class When_a_remote_actor_is_referenced :
		Given_a_loopback_registry
	{
		[Then]
		public void Should_allow_selection_of_a_remote_actor()
		{
			Registry.Select(new ActorUrn(Address, ActorId), actor =>
			{
				actor.Send(new A
				{
					Name = "bob"
				});
			}, () => {});

			ReceivedA.WaitUntilCompleted(2.Seconds()).ShouldBeTrue();
		}
	}
}