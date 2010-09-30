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
	using System.Linq;
	using Channels;
	using Magnum.Extensions;
	using ForNHibernate.Auditing;
	using Stact.StateMachine;
	using Magnum.StateMachine;


	public class TestInstanceAudit :
		StateMachine<TestInstanceAudit>
	{
		static TestInstanceAudit()
		{
			Define(Definition);
		}

		public TestInstanceAudit(AuditKey id)
		{
			Id = id;
		}

		public TestInstanceAudit()
		{
		}

		public virtual AuditKey Id { get; private set; }
		public virtual string UserName { get; set; }
		public virtual decimal? Value { get; set; }
		public virtual decimal? OriginalValue { get; set; }

		public static Event<PreInsertEvent<TestInstance>> Inserted { get; set; }

		public static State Initial { get; set; }
		public static State Completed { get; set; }


		static void Definition()
		{
			Initially(
			          When(Inserted)
			          	.Then((audit, msg) =>
			          		{
			          			msg.Changes.Where(x => x.Name == "Value").
			          				Each(x =>
			          					{
											if(x.Value != null)
			          							audit.Value = (decimal)x.Value;
											if(x.OriginalValue != null)
			          							audit.OriginalValue = (decimal)x.OriginalValue;
			          					});
			          			audit.UserName = msg.Identity.Name;
			          		})
			          	.Complete());
		}
	}
}