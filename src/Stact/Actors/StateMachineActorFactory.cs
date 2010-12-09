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
	using Workflow;
	using Workflow.Configuration;


	public static class StateMachineActorFactory
	{
		public static ActorFactory<TActor> Create<TWorkflow, TActor>(Func<Inbox, TActor> createInstance,
		                                                             Action<StateMachineConfigurator<TWorkflow, TActor>>
		                                                             	configurationAction)
			where TActor : class, Actor
			where TWorkflow : class
		{
			StateMachineWorkflow<TWorkflow, TActor> workflow = StateMachineWorkflow.New(configurationAction);

			var workflowBinder = new StateMachineWorkflowBinder<TWorkflow, TActor>(workflow);

			ActorFactory<TActor> factory = ActorFactory.Create<TActor>(x =>
				{
					x.ConstructedBy(inbox =>
						{
							TActor instance = createInstance(inbox);

							workflowBinder.Bind(inbox, instance);

							return instance;
						});
				});

			return factory;
		}
	}
}