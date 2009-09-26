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
namespace Magnum.Infrastructure.Specs.StateMachine
{
	using System;
	using System.Data;
	using Infrastructure.Data;
	using Magnum.Data;
	using Magnum.Specs.StateMachine;
	using NUnit.Framework;
	using NHibernate;
	using NHibernate.Cfg;
	using NHibernate.Tool.hbm2ddl;

	[TestFixture]
	public class StateMachineStorage_Specs
	{
		[SetUp]
		public void Setup()
		{
			_cfg = new Configuration();

			_cfg.SetProperty("connection.provider", "NHibernate.Connection.DriverConnectionProvider");
			_cfg.SetProperty("connection.driver_class", "NHibernate.Driver.SqlClientDriver");
			_cfg.SetProperty("connection.connection_string", _connectionString);
			_cfg.SetProperty("dialect", "NHibernate.Dialect.MsSql2005Dialect");
			_cfg.SetProperty("default_schema", "bus");

			_cfg.AddAssembly(typeof (StateMachineStorage_Specs).Assembly);

			ISessionFactory _sessionFactory = _cfg.BuildSessionFactory();

			LocalContext.Current.Store(_sessionFactory);

			NHibernateUnitOfWork.SetSessionProvider(() => LocalContext.Current.Retrieve<ISessionFactory>().OpenSession());

			UnitOfWork.SetUnitOfWorkProvider(NHibernateUnitOfWork.Create);
		}

		[TearDown]
		public void Teardown()
		{
			NHibernateUnitOfWork.SetSessionProvider(null);

			LocalContext.Current.Clear();
		}

		private const string _connectionString = "Server=localhost;initial catalog=test;Trusted_Connection=yes";
		private Configuration _cfg;

		[Test, Explicit]
		public void First_we_need_a_schema_to_test()
		{
			new SchemaExport(_cfg).Create(true, true);
		}

		[Test, Explicit]
		public void Saving_a_state_machine_should_work()
		{
			Guid transactionId;

			using (var unitOfWork = UnitOfWork.Start())
			using (var transaction = unitOfWork.BeginTransaction(IsolationLevel.Serializable))
			{
				using(var repository = new NHibernateRepository() )
				{
					ExampleStateMachine machine = new ExampleStateMachine();

					repository.Save(machine);

					transactionId = machine.TransactionId;
				}

				transaction.Commit();
			}
			UnitOfWork.Finish();

			using (var unitOfWork = UnitOfWork.Start())
			using (var transaction = unitOfWork.BeginTransaction(IsolationLevel.Serializable))
			{
				using (var repository = new NHibernateRepository())
				{
					ExampleStateMachine machine = repository.Get<ExampleStateMachine>(transactionId);

					machine.SubmitOrder();

					repository.Update(machine);
				}

				transaction.Commit();
			}
			UnitOfWork.Finish();


		}
	}
}