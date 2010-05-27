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
namespace Sample.Actors
{
	using Magnum.Actors;
	using Magnum.Channels;
	using Magnum.Fibers;
	using Messages;

	public class Auctioneer :
		Actor
	{
		private readonly Fiber _fiber;
		private readonly ActorRepository<Seller, string> _sellerRepository;
		private UntypedChannelAdapter _input;
		private DefaultMailbox<RegisterSeller> _registerSeller;
		private ChannelSubscription _subscriptions;

		public Auctioneer(Fiber fiber, Scheduler scheduler, ActorRepository<Seller, string> sellerRepository)
		{
			_fiber = fiber;
			_sellerRepository = sellerRepository;

			_registerSeller = new DefaultMailbox<RegisterSeller>(fiber, scheduler);

			_input = new UntypedChannelAdapter(fiber);

			_subscriptions = _input.Subscribe(x =>
				{
					x.Consume<RegisterSeller>()
						.Using(message => _registerSeller.Send(message));
				});

			_registerSeller.Receive(OnRegisterSeller);
		}

		private Consumer<RegisterSeller> OnRegisterSeller(RegisterSeller message)
		{
			return m =>
				{
					var seller = _sellerRepository.Get(m.Name, key => new Seller(key));
				};
		}

		public void Send<T>(T message)
		{
			_input.Send(message);
		}

		public void Send<T>(T message, RequestResponseChannel replyTo)
		{
			_input.Send(message);
		}
	}
}