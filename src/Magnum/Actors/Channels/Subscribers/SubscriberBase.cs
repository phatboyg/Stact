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
namespace Magnum.Actors.Channels.Subscribers
{
	public delegate bool Filter<T>(T message);

	public abstract class SubscriberBase<T>
	{
		private readonly Filter<T> _filter;

		protected SubscriberBase()
		{
		}

		protected SubscriberBase(Filter<T> filter)
		{
			_filter = filter;
		}

		public void Consume(T message)
		{
			if (IsSatisfiedByFilter(message))
			{
				ConsumeMessage(message);
			}
		}

		protected abstract void ConsumeMessage(T message);

		private bool IsSatisfiedByFilter(T msg)
		{
			return _filter == null || _filter(msg);
		}
	}
}