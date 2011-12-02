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
namespace Stact.Specs.Actors.Auctions
{
    using System;
    
    using Magnum;
    using Stact;
    using Magnum.Extensions;


    public class Auction 
    {
        readonly Fiber _fiber;
        readonly ActorInbox _inbox;
        decimal _currentBid = 1.00m;
        bool _ended;

        public Auction(Fiber fiber, ActorInbox inbox, Guid id)
        {
            Id = id;
            _inbox = inbox;
            _fiber = fiber;

            AskChannel = new SelectiveConsumerChannel<Message<Ask>>(_fiber, HandleAsk);
            EndChannel = new ConsumerChannel<End>(_fiber, x => { _ended = true; });
        }

        public Guid Id { get; private set; }
        public Channel<Message<Ask>> AskChannel { get; private set; }
        public Channel<End> EndChannel { get; private set; }

        Consumer<Message<Ask>> HandleAsk(Message<Ask> x)
        {
            if (x.Body.AuctionId != Id)
                return null;

            return message =>
                {
                    if (_ended)
                    {
                        x.Respond(new Ended
                            {
                                AuctionId = Id,
                                HighBid = _currentBid,
                            });
                        return;
                    }

                    Guid token = CombGuid.Generate();

                    x.Respond(new Status
                        {
                            CurrentBid = _currentBid,
                            AuctionId = Id,
                            Token = token,
                        });

                    _inbox.Receive<Buy>(buy =>
                        {
                            if (buy.Body.Token != token)
                                return null;

                            return buyMessage =>
                                {
                                    buyMessage.Respond(new Purchased
                                        {
                                            AuctionId = Id,
                                            Token = token,
                                            Price = _currentBid,
                                            Quantity = buy.Body.Quantity,
                                        });
                                };

                        }, 1.Seconds(),
                        () =>
                            {
                                // nothing to do on timeout
                            });
                };
        }
    }

    public class Purchased
    {
        public Guid AuctionId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public Guid Token { get; set; }
    }

    public class Buy
    {
        public Buy(Guid token, int quantity)
        {
            Token = token;
            Quantity = quantity;
        }

        public Buy()
        {
        }

        public Guid Token { get; set; }
        public int Quantity { get; set; }
    }

    public class End
    {

    }

    public class Ask
    {
        public Ask(Guid id)
        {
            AuctionId = id;
        }

        public Guid AuctionId { get; set; }
    }


    public class Ended
    {
        public Guid AuctionId { get; set; }
        public decimal HighBid { get; set; }
    }

    public class Status
    {
        public Guid AuctionId { get; set; }
        public decimal CurrentBid { get; set; }

        public Guid Token { get; set; }
    }
}