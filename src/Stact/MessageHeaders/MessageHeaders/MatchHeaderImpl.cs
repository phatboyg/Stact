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
namespace Stact.MessageHeaders
{
	using System;
	using System.Collections.Generic;


	public class MatchHeaderImpl :
		MatchHeader
	{
		readonly IDictionary<Type, Action<object, MatchHeaderCallback>> _adapters;
		readonly HashSet<Type> _directTypes;

		public MatchHeaderImpl()
		{
			_adapters = new Dictionary<Type, Action<object, MatchHeaderCallback>>();
			_directTypes = new HashSet<Type>();
		}

		public void Match<TInput>(TInput input, MatchHeaderCallback callback)
		{
			if (_directTypes.Contains(typeof(TInput)))
			{
				callback.Body(input);
				return;
			}

			Action<object, MatchHeaderCallback> adapter;
			if (!_adapters.TryGetValue(typeof(TInput), out adapter))
			{
				foreach (MatchHeaderSelectorFactory factory in GetTypeConverters(typeof(TInput)))
				{
					if (factory.CanMatch(input, out adapter))
						break;
				}

				if (adapter == null)
					_directTypes.Add(typeof(TInput));
				else
					_adapters.Add(typeof(TInput), adapter);
			}

			if (adapter == null)
				callback.Body(input);
			else
				adapter(input, callback);
		}

		static IEnumerable<MatchHeaderSelectorFactory> GetTypeConverters(Type messageType)
		{
			yield return new RequestSelectorFactory(messageType);
			yield return new ResponseSelectorFactory(messageType);
			yield return new MessageSelectorFactory(messageType);
		}
	}
}