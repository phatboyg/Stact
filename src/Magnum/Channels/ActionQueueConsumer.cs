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
	using Actions;

	public class ActionQueueConsumer<T>
	{
		private readonly ThreadPoolActionQueue _queue;

		public ActionQueueConsumer(Consumer<T> consumer)
		{
			_queue = new ThreadPoolActionQueue();

			Channel = new ConsumerChannel<T>(_queue, consumer);
		}

		public Channel<T> Channel { get; private set; }
	}
}