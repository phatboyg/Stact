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
namespace Stact.Routing
{
	using Internal;


	public class SelectiveConsumerNode<TChannel> :
		Activation<TChannel>
	{
		readonly SelectiveConsumer<TChannel> _selectiveConsumer;
		bool _alive = true;

		public SelectiveConsumerNode(SelectiveConsumer<TChannel> selectiveConsumer)
		{
			_selectiveConsumer = selectiveConsumer;
		}

		public void Activate(RoutingContext<TChannel> context)
		{
			Consumer<TChannel> consumer = _selectiveConsumer(context.Body);
			if (_selectiveConsumer == null)
				return;

			var body = context.Body;
			context.Evict();

			context.Add(() => consumer(body));

			_alive = false;
		}

		public bool IsAlive
		{
			get { return _alive; }
		}
	}
}