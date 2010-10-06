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
namespace Stact.ForNHibernate.Specs.Channels
{
	using FluentNHibernate.Mapping;
	using Magnum.ForNHibernate.StateMachine;
	using Stact.Specs.StateMachine;
	using StateMachine;


	public class TestStateMachineInstanceMap :
		ClassMap<TestStateMachineInstance>
	{
		public TestStateMachineInstanceMap()
		{
			Not.LazyLoad();

			Id(x => x.Id)
				.GeneratedBy.Assigned();

			Version(x => x.LastUpdatedAt);

			Map(x => x.Value);
			Map(x => x.CreatedAt);
			Map(x => x.UpdatedAt);
			Map(x => x.CompletedAt);

			Map(x => x.CurrentState)
				.Access.ReadOnlyPropertyThroughCamelCaseField(Prefix.Underscore)
				.CustomType<StateMachineUserType>();
		}
	}
}