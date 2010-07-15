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
namespace Magnum.Channels
{
	using System;


	/// <summary>
	/// A direct delegate invocation on send, no fiber used
	/// </summary>
	/// <typeparam name="TChannel"></typeparam>
	public class DelegateChannel<TChannel> :
		Channel<TChannel>
	{
		Action<TChannel> _handler;

		public DelegateChannel(Action<TChannel> handler)
		{
			_handler = handler;
		}

		public void Send(TChannel message)
		{
			_handler(message);
		}
	}
}