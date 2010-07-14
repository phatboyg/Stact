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
	using System.Linq;
	using Magnum.Channels;
	using Magnum.Channels.Configuration;
	using Magnum.Channels.Configuration.Internal;
	using Magnum.Extensions;
	using Magnum.Fibers;


	public class StateMachineInstanceConnectionConfiguratorImpl<T> :
		FiberModelConfigurator<StateMachineInstanceConnectionConfigurator<T>>,
		ChannelConfigurator,
		StateMachineInstanceConnectionConfigurator<T>
		where T : StateMachine<T>
	{
		readonly ConnectionConfigurator _configurator;
		readonly T _instance;
		StateMachineEventInspectorResult<T>[] _results;

		public StateMachineInstanceConnectionConfiguratorImpl(ConnectionConfigurator configurator, T instance)
		{
			_configurator = configurator;
			_instance = instance;

			_configurator.RegisterChannelConfigurator(this);
		}

		public void ValidateConfiguration()
		{
			if (_instance == null)
				throw new ChannelConfigurationException("State machine instance cannot be null: " + typeof(T).ToShortTypeName());

			var inspector = new StateMachineEventInspector<T>();
			_instance.Inspect(inspector);

			_results = inspector.GetResults().ToArray();
		}

		public void Configure(CreateChannelConnection connection, UntypedChannel channel)
		{
			Fiber fiber = _fiberFactory();

			foreach (var result in _results)
				result.Connect(_configurator, fiber, _instance);
		}
	}
}