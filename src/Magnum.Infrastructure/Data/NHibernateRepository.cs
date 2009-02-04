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
	using System.Collections.Generic;
	using System.Linq;
	using Magnum.Data;
	using NHibernate;
	using NHibernate.Linq;

	public class NHibernateRepository :
		IRepository
	{
		private readonly Func<ISession> _session;

		public NHibernateRepository()
		{
			_session = () => NHibernateUnitOfWork.Current.Session;
		}

		public NHibernateRepository(ISession session)
		{
			_session = () => session;
		}

		public void Dispose()
		{}

		public T Get<T>(object id) where T : class
		{
			return _session().Get<T>(id);
		}

		public IList<T> List<T>() where T : class
		{
			return _session().CreateCriteria(typeof (T)).List<T>();
		}

		public void Save<T>(T item) where T : class
		{
			_session().Save(item);
		}

		public void Update<T>(T item) where T : class
		{
			_session().Update(item);
		}

		public void Delete<T>(T item) where T : class
		{
			_session().Delete(item);
		}

        public IQueryable<T> FindBy<T>() where T : class
        {
            return _session().Linq<T>();
        }
	}

	public class NHibernateRepository<T, K> :
		RepositoryBase<T, K>
		where T : class, IAggregateRoot<K>
	{
		private readonly object _queryLock = new object();
		private readonly ISession _session;

		public NHibernateRepository()
		{
			_session = NHibernateUnitOfWork.Current.Session;
		}

		public NHibernateRepository(ISession session)
		{
			_session = session;
		}

		protected override IQueryable<T> RepositoryQuery
		{
			get
			{
				lock (_queryLock)
				{
					return _session.Linq<T>();
				}
			}
		}

		public override void Dispose()
		{
		}

		public override T Get(K id)
		{
			return _session.Get<T>(id);
		}

		public override IList<T> List()
		{
			return _session.CreateCriteria(typeof (T)).List<T>();
		}

		public override void Save(T item)
		{
			_session.Save(item);
		}

		public override void Update(T item)
		{
			_session.Update(item);
		}

		public override void Delete(T item)
		{
			_session.Delete(item);
		}
	}
}