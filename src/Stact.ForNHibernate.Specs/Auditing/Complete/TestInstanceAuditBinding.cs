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
namespace Stact.ForNHibernate.Specs.Auditing.Complete
{
	using Channels;
	using ForNHibernate.Auditing;
	using Magnum.StateMachine;
	using Stact.StateMachine;


	public class TestInstanceAuditBinding :
		StateMachineBinding<TestInstanceAudit, AuditKey>
	{
		public TestInstanceAuditBinding()
		{
			Id(x => x.Id);

			Bind(TestInstanceAudit.Inserted, GetKey);
		}

		AuditKey GetKey(EntityAuditEvent<TestInstance> e)
		{
			return new AuditKey
				{
					TestInstanceId = e.Entity.Id,
					SessionId = e.SessionId,
					Timestamp = e.Timestamp,
				};
		}
	}
}