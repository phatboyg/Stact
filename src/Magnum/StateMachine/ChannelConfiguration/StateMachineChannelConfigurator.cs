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
namespace Magnum.StateMachine.ChannelConfiguration
{
	using Channels.Configuration;
	using Fibers;


	public class StateMachineConnectionConfigurator<T>
		where T : StateMachine<T>
	{
		readonly ConnectionConfigurator _configurator;

		public StateMachineConnectionConfigurator(ConnectionConfigurator configurator)
		{
			_configurator = configurator;
		}

		public void Connect(Fiber fiber, T instance)
		{
			var inspector = new StateMachineEventInspector<T>();
			instance.Inspect(inspector);

			foreach (var result in inspector.GetResults())
				result.Connect(_configurator, fiber, instance);
		}
	}
}