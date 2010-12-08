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
namespace Stact.Configuration.Internal
{
	using System;


	public class UntypedChannnelConnectionBuilder :
		ConnectionBuilder
	{
		readonly UntypedChannel _channel;
		readonly UntypedChannelConnection _connection;

		public UntypedChannnelConnectionBuilder(UntypedChannel channel)
		{
			_connection = new UntypedChannelConnection(channel);
			_channel = channel;
		}

		public ChannelConnection ChannelConnection
		{
			get { return _connection; }
		}

		public void AddChannel(Fiber fiber, Func<Fiber, UntypedChannel> channelFactory)
		{
			UntypedChannel channel = channelFactory(fiber);

			ConnectChannel(channel);
		}

		public void AddChannel<TChannel>(Fiber fiber, Func<Fiber, Channel<TChannel>> channelFactory)
		{
			Channel<TChannel> channel = channelFactory(fiber);

			ConnectChannel(channel);
		}

		public UntypedChannel Channel
		{
			get { return _channel; }
		}

		public void AddDisposable(IDisposable disposable)
		{
			_connection.AddDisposable(disposable);
		}

		void ConnectChannel<T>(Channel<T> channel)
		{
			new ConnectChannelVisitor<T>(channel).ConnectTo(_channel);

			_connection.AddChannel(channel);
		}

		void ConnectChannel(UntypedChannel channel)
		{
			new ConnectChannelVisitor(channel).ConnectTo(_channel);

			_connection.AddChannel(channel);
		}
	}
}