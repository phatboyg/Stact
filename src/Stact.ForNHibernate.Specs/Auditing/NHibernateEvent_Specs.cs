namespace Stact.ForNHibernate.Specs.Auditing
{
	using Channels;
	using NHibernate;
	using NUnit.Framework;
	using TestFramework;


	[Scenario]
	[Category("Database")]
	public class Performing_a_single_entity_delete :
		Given_an_nhibernate_session_factory
	{
		[When]
		public void A_single_row_is_deleted()
		{
			using (ISession session = SessionFactory.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.CreateQuery("Delete TestInstance").ExecuteUpdate();

				session.Save(new TestInstance(27));

				transaction.Commit();
			}

			ResetSessionFactory();
			ExtraConfiguration = cfg =>
			{
			};

			using (ISession session = SessionFactory.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var instance = session.Get<TestInstance>(27);
				if(instance != null)
					session.Delete(instance);

				transaction.Commit();
			}
		}
	}
}