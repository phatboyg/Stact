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
namespace Stact.Routing.Configuration.Internal
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Magnum.Extensions;


	public class DynamicRoutingEngineConfigurator :
		RoutingEngineConfigurator
	{
		readonly IList<RoutingConfigurator> _configurators;
		readonly DynamicRoutingEngine _engine;

		public DynamicRoutingEngineConfigurator(DynamicRoutingEngine engine)
		{
			_engine = engine;
			_configurators = new List<RoutingConfigurator>();
		}

		public IEnumerator<RoutingConfigurator> GetEnumerator()
		{
			return _configurators.GetEnumerator();
		}

		public void Add(RoutingConfigurator configurator)
		{
			_configurators.Add(configurator);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public RoutingEngineConfiguration Compile()
		{
			ValidateConfiguration();

			var context = new DynamicRoutingEngineConfiguratorContext(_engine);

			_configurators.Each(x => x.Configure(_engine));

			throw new NotImplementedException();
		}

		void ValidateConfiguration()
		{
			_configurators.Each(x => x.ValidateConfiguration());
		}
	}


	public class DynamicRoutingEngineConfiguratorContext
	{
		readonly DynamicRoutingEngine _engine;

		public DynamicRoutingEngineConfiguratorContext(DynamicRoutingEngine engine)
		{
			_engine = engine;
		}
	}
}