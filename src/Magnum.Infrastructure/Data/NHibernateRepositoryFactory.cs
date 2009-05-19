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
namespace Magnum.Infrastructure.Data
{
	using System;
	using Magnum.Data;
	using NHibernate;
	using NHibernate.Cfg;

	public class NHibernateRepositoryFactory : IRepositoryFactory
	{
		private readonly ISessionFactory _sessionFactory;

		public NHibernateRepositoryFactory(ISessionFactory sessionFactory)
		{
			_sessionFactory = sessionFactory;
		}

		public void Dispose()
		{
			_sessionFactory.Dispose();
		}

		public IRepository GetRepository()
		{
			return new NHibernateRepository(_sessionFactory.OpenSession());
		}

		public IRepository<T> GetRepository<T>() where T : class
		{
			return new NHibernateRepository<T>(_sessionFactory.OpenSession());
		}

		public static IRepositoryFactory Configure(Action<Configuration> configure)
		{
			Configuration configuration = new Configuration().Configure();

			configuration.AddAssembly(typeof (NHibernateRepositoryFactory).Assembly);

			configure(configuration);

			return new NHibernateRepositoryFactory(configuration.BuildSessionFactory());
		}
	}
}