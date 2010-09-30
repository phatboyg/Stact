// Copyright 2007-2010 The Apache Software Foundation.
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
	using ForNHibernate.Auditing;
	using Stact.Channels;
	using NHibernate;
	using NHibernate.Linq;
	using NUnit.Framework;
	using Magnum.TestFramework;
	using Magnum.StateMachine;


	[Scenario]
	[Category("Database")]
	public class When_an_instance_is_changed_and_an_audit_state_machine_is_used :
		Given_an_nhibernate_session_factory
	{
		ChannelAdapter _channel;
		ChannelConnection _connection;

		[When]
		public void Configuring_event_listeners_for_nhibernate()
		{
			using (ISession session = SessionFactory.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.CreateQuery("Delete TestInstance").ExecuteUpdate();
				session.CreateQuery("Delete TestInstanceAudit").ExecuteUpdate();

				transaction.Commit();
			}

			_channel = new ChannelAdapter();
			_connection = _channel.Connect(x =>
				{
					x.AddConsumersFor<TestInstanceAudit>()
						.BindUsing<TestInstanceAuditBinding, AuditKey>()
						.HandleOnCallingThread()
						.CreateNewInstanceBy(id => new TestInstanceAudit(id))
						.PersistUsingNHibernate()
						.UseSessionProvider(() => SessionFactory.OpenSession());
				});

			ResetSessionFactory();
			ExtraConfiguration = cfg => { cfg.AddAuditEventListeners(_channel); };

			using (ISession session = SessionFactory.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.CreateQuery("Delete TestInstance").ExecuteUpdate();

				var instance = new TestInstance(27, 123.45m);
				session.Save(instance);
				instance = new TestInstance(37, 123.45m);
				session.Save(instance);
				instance = new TestInstance(47, 123.45m);
				session.Save(instance);

				transaction.Commit();
			}
		}

		[Finally]
		public void Close_connection()
		{
			if (_connection != null)
			{
				_connection.Dispose();
				_connection = null;
			}
		}

		[Then]
		public void Should_create_the_audit_instance_for_each_insert()
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				session.Linq<TestInstanceAudit>().Count().ShouldEqual(3);
			}
		}

		[Then]
		public void Should_create_the_audit_instance()
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				TestInstanceAudit instance = session.Linq<TestInstanceAudit>().First(x => x.Id.TestInstanceId == 27);

				instance.Value.HasValue.ShouldBeTrue();
				instance.Value.Value.ShouldEqual(123.45m);
				instance.OriginalValue.HasValue.ShouldBeFalse();
			}
		}
	}
}