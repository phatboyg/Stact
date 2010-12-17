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
namespace Stact
{
	using System;
	using Magnum;
	using Magnum.Extensions;


	public class DelegateInstanceProvider<TInstance, TChannel> :
		InstanceProvider<TInstance, TChannel>
		where TInstance : class
	{
		readonly Func<TChannel, TInstance> _provider;

		public DelegateInstanceProvider(Func<TChannel, TInstance> provider)
		{
			Guard.AgainstNull(provider);

			_provider = provider;
		}

		public TInstance GetInstance(TChannel message)
		{
			TInstance instance = _provider(message);
			if (instance == null)
			{
				throw new InvalidOperationException(
					"The instance of type {0} was null for the message type {1}".FormatWith(typeof(TInstance).ToShortTypeName(),
					                                                                        typeof(TChannel).ToShortTypeName()));
			}

			return instance;
		}
	}
}