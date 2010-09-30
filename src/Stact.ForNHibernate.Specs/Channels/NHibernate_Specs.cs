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
	using System;
	using FluentNHibernate.Cfg;
	using Magnum.ForNHibernate.StateMachine;
	using NHibernate;
	using NHibernate.Cfg;
	using NHibernate.Tool.hbm2ddl;
	using NUnit.Framework;
	using Magnum.TestFramework;


	[Scenario]
	[Category("Database")]
	public class Given_an_nhibernate_session_factory
	{
		static ISessionFactory _sessionFactory;

		public ISessionFactory SessionFactory
		{
			get
			{
				if (_sessionFactory == null)
					_sessionFactory = CreateSessionFactory();

				return _sessionFactory;
			}
		}

		protected Action<Configuration> ExtraConfiguration { get; set; }

		[Given]
		public void An_nhibernate_session_factory()
		{
		}

		[Finally]
		public void Afterwards_man()
		{
			ResetSessionFactory();
		}

		protected void ResetSessionFactory()
		{
			if (_sessionFactory != null)
			{
				_sessionFactory.Dispose();
				_sessionFactory = null;
			}
		}

		protected ISessionFactory CreateSessionFactory()
		{
			return Fluently.Configure()
				.Mappings(m =>
					{
						m.FluentMappings
							.Conventions
							.Add<StateMachineUserTypeConvention>()
							.AddFromAssemblyOf<Given_an_nhibernate_session_factory>();
					})
				.ExposeConfiguration(cfg =>
					{
						new SchemaExport(cfg).Create(false, true);

						if (ExtraConfiguration != null)
							ExtraConfiguration(cfg);
					})
				.BuildSessionFactory();
		}
	}


	[Scenario]
	[Category("Database")]
	public class Given_an_empty_test_instance_table :
		Given_an_nhibernate_session_factory
	{
		[Given]
		public void An_empty_test_instance_table()
		{
			using (ISession session = SessionFactory.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.CreateQuery("Delete TestInstance").ExecuteUpdate();


				transaction.Commit();
			}
		}
	}
}