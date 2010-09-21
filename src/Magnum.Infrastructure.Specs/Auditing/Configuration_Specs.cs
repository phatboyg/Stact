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
namespace Magnum.Infrastructure.Specs.Auditing
{
	using Channels;
	using Infrastructure.Auditing;
	using Magnum.Channels;
	using NHibernate;
	using NHibernate.Cfg;
	using TestFramework;


	[Scenario]
	public class When_configuring_event_listeners_for_nhibernate :
		Given_an_nhibernate_session_factory
	{
		ChannelAdapter _auditChannel;
		Future<PostUpdateEvent<TestInstance>> _postUpdate;
		Future<PreUpdateEvent<TestInstance>> _preUpdate;

		protected override void OnExposeConfiguration(Configuration cfg)
		{
			cfg.AddAuditEventListeners(_auditChannel);
		}

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
				instance.Value = 123.45m;

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
	}
}