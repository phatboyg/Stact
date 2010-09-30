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
namespace Stact.ForNHibernate.Specs.Auditing
{
	using Channels;
	using ForNHibernate.Auditing;
	using Stact.Channels;
	using NHibernate;
	using NUnit.Framework;
	using TestFramework;


	[Scenario]
	[Category("Database")]
	public class When_configuring_event_listeners_for_nhibernate :
		Given_an_nhibernate_session_factory
	{
		ChannelAdapter _auditChannel;
		Future<PostUpdateEvent<TestInstance>> _postUpdate;
		Future<PreUpdateEvent<TestInstance>> _preUpdate;

		[When]
		public void Configuring_event_listeners_for_nhibernate()
		{
			_auditChannel = new ChannelAdapter();

			_preUpdate = new Future<PreUpdateEvent<TestInstance>>();
			_postUpdate = new Future<PostUpdateEvent<TestInstance>>();

			_auditChannel.Connect(x =>
				{
					x.AddConsumerOf<PreUpdateEvent<TestInstance>>()
						.UsingConsumer(_preUpdate.Complete)
						.HandleOnCallingThread();

					x.AddConsumerOf<PostUpdateEvent<TestInstance>>()
						.UsingConsumer(_postUpdate.Complete)
						.HandleOnCallingThread();
				});

			ExtraConfiguration = cfg =>
				{
					cfg.AddAuditEventListeners(_auditChannel);
				};

			using (ISession session = SessionFactory.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.CreateQuery("Delete TestInstance").ExecuteUpdate();

				session.Save(new TestInstance(27));

				transaction.Commit();
			}

			using (ISession session = SessionFactory.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var instance = session.Get<TestInstance>(27);
				instance.UpdateValueChannel.Send(new UpdateValue(27, 123.45m));

				transaction.Commit();
			}
		}

		[Then]
		public void Should_call_the_pre_update_event_handler()
		{
			_preUpdate.IsCompleted.ShouldBeTrue();
		}

		[Then]
		public void Should_call_the_post_update_event_handler()
		{
			_postUpdate.IsCompleted.ShouldBeTrue();
		}

		[Test]
		public void Should_have_one_previous_value()
		{
			TestInstance instance;

			using (ISession session = SessionFactory.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				instance = session.Get<TestInstance>(27);

				transaction.Commit();
			}

			instance.Value.ShouldEqual(123.45m);
			instance.PreviousValues.ShouldNotBeNull();
			instance.PreviousValues.Count.ShouldEqual(1);
			instance.PreviousValues[0].Value.ShouldEqual(0.0m);

		}
	}
}