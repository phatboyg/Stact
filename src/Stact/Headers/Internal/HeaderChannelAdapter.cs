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
namespace Stact.Internal
{
	using System;
	using System.Collections.Generic;


	public class HeaderChannelAdapter :
		UntypedChannel
	{
		readonly IDictionary<Type, Action<object, HeaderChannel>> _adapters;
		readonly HashSet<Type> _directTypes;
		readonly HeaderChannel _output;

		public HeaderChannelAdapter(HeaderChannel output)
		{
			_output = output;

			_adapters = new Dictionary<Type, Action<object, HeaderChannel>>();
			_directTypes = new HashSet<Type>();
		}

		public void Send<TInput>(TInput input)
		{
			Console.WriteLine("Sending type: " + typeof(TInput).FullName);

			if (_directTypes.Contains(typeof(TInput)))
			{
				_output.Send(input);
				return;
			}

			Action<object, HeaderChannel> adapter;
			if (!_adapters.TryGetValue(typeof(TInput), out adapter))
			{
				foreach (HeaderChannelAdapterFactory factory in GetTypeConverters(typeof(TInput)))
				{
					if (factory.CanAdapt(input, out adapter))
						break;
				}

				if (adapter == null)
					_directTypes.Add(typeof(TInput));
				else
					_adapters.Add(typeof(TInput), adapter);
			}

			if (adapter == null)
				_output.Send(input);
			else
				adapter(input, _output);
		}

		static IEnumerable<HeaderChannelAdapterFactory> GetTypeConverters(Type messageType)
		{
			yield return new RequestAdapterFactory(messageType);
			yield return new ResponseAdapterFactory(messageType);
			yield return new MessageAdapterFactory(messageType);
		}
	}
}