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
		readonly HeaderChannel _output;
		IDictionary<Type, Action<object, HeaderChannel>> _adapters;

		HashSet<Type> _ignoredTypes;

		public HeaderChannelAdapter(HeaderChannel output)
		{
			_output = output;
		}

		public void Send<TInput>(TInput input)
		{
			if (_adapters == null)
				_adapters = new Dictionary<Type, Action<object, HeaderChannel>>();

			Action<object, HeaderChannel> adapter;
			if (!_adapters.TryGetValue(typeof(TInput), out adapter))
			{
				if (_ignoredTypes == null)
					_ignoredTypes = new HashSet<Type>();

				if (_ignoredTypes.Contains(typeof(TInput)))
					return;

				foreach (HeaderChannelAdapterFactory factory in GetTypeConverters(typeof(TInput)))
				{
					if (factory.CanAdapt(input, out adapter))
						break;
				}

				if (adapter == null)
				{
					_ignoredTypes.Add(typeof(TInput));
					return;
				}

				_adapters.Add(typeof(TInput), adapter);
			}

			if (adapter == null)
				return;

			adapter(input, _output);
		}

		static IEnumerable<HeaderChannelAdapterFactory> GetTypeConverters(Type messageType)
		{
			yield return new RequestRouterFactory(messageType);
			yield return new ResponseRouterFactory(messageType);
			yield return new MessageRouterFactory(messageType);
		}
	}
}