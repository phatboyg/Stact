namespace Stact.ForNHibernate.Specs.Auditing
{
	using System.Linq;
	using Channels;
	using ForNHibernate.Auditing;
	using ForNHibernate.Auditing.Internal;
	using Stact.Channels;
	using NHibernate;
	using NUnit.Framework;
	using Magnum.TestFramework;


	[Scenario]
	[Category("Database")]
	public class When_capturing_a_pre_insert_event_for_an_object :
		Given_an_nhibernate_session_factory
	{
		ChannelAdapter _channel;
		Future<PreInsertEvent<TestInstance>> _preInsert;
		Future<PreInsertEvent<PreviousValue>> _preInsertPrevious;

		[When]
		public void Configuring_event_listeners_for_nhibernate()
		{
			_channel = new ChannelAdapter();

			_preInsert = new Future<PreInsertEvent<TestInstance>>();
			_preInsertPrevious = new Future<PreInsertEvent<PreviousValue>>();

			_channel.Connect(x =>
			{
				x.AddConsumerOf<PreInsertEvent<TestInstance>>()
					.UsingConsumer(_preInsert.Complete)
					.HandleOnCallingThread();

				x.AddConsumerOf<PreInsertEvent<PreviousValue>>()
					.UsingConsumer(_preInsertPrevious.Complete)
					.HandleOnCallingThread();
			});

			ExtraConfiguration = cfg =>
			{
				cfg.AddAuditEventListeners(_channel);
			};

			using (ISession session = SessionFactory.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.CreateQuery("Delete TestInstance").ExecuteUpdate();

				var instance = new TestInstance(27, 123.45m);
				instance.UpdateValueChannel.Send(new UpdateValue(27, 100.0m));

				session.Save(instance);

				transaction.Commit();
			}
		}

		[Then]
		public void Should_call_the_pre_update_event_handler()
		{
			_preInsert.IsCompleted.ShouldBeTrue();
		}

		[Then]
		public void Should_call_the_update_for_the_collection_type()
		{
			_preInsertPrevious.IsCompleted.ShouldBeTrue();
		}

		[Test]
		public void Should_have_the_entity_value_present()
		{
			_preInsert.Value.Entity.ShouldNotBeNull();
		}

		[Test]
		public void Should_have_the_proper_change_count()
		{
			_preInsert.Value.Changes.Count().ShouldEqual(2);

			PropertyChange change = _preInsert.Value.Changes.Single(x => x.Name == "Value");
			change.ShouldNotBeNull();

			change.Name.ShouldEqual("Value");
			change.Value.ShouldEqual(223.45m);
			change.OriginalValue.ShouldEqual(null);
		}
	}
}