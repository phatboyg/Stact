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
namespace Stact.ForNHibernate.Specs.Channels
{
	using System;
	using Stact.Channels;
	using NHibernate;
	using NUnit.Framework;
	using TestFramework;


	[Scenario]
	public class When_sending_a_message_to_an_nhibernate_instance_channel :
		Given_an_nhibernate_session_factory
	{
		decimal _newValue;

		[When]
		public void Sending_a_message_to_an_nhibernate_instance_channel()
		{
			_newValue = new Random().Next(1, 500000)/100m;

			using (ISession session = SessionFactory.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.CreateQuery("Delete TestInstance").ExecuteUpdate();

				session.Save(new TestInstance(27));

				transaction.Commit();
			}

			var input = new ChannelAdapter();
			using (input.Connect(x =>
				{
					x.AddConsumerOf<UpdateValue>()
						.UsingInstance()
						.Of<TestInstance>()
						.DistributedBy(msg => msg.Id)
						.HandleOnCallingThread()
						.PersistUsingNHibernate()
						.UsingSessionProvider(m => SessionFactory.OpenSession())
						.OnChannel(m => m.UpdateValueChannel)
						.CreateNewInstanceBy(m => new TestInstance(m.Id));
				}))
			{
				//
				input.Send(new UpdateValue(27, _newValue));
			}
		}

		[Then]
		[Category("Database")]
		public void Should_load_the_matching_instance_and_send_it_the_message()
		{
			using (ISession session = SessionFactory.OpenSession())
				session.Load<TestInstance>(27).ShouldNotBeNull();
		}

		[Then]
		[Category("Database")]
		public void Should_not_contain_a_missing_instance()
		{
			using (ISession session = SessionFactory.OpenSession())
				Assert.Throws<ObjectNotFoundException>(() => session.Load<TestInstance>(47));
		}

		[Then]
		[Category("Database")]
		public void Should_have_updated_the_value_on_the_instance()
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				var instance = session.Load<TestInstance>(27);
				instance.ShouldNotBeNull();
				instance.Value.ShouldEqual(_newValue);
			}
		}
	}
}