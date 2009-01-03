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
namespace Magnum.Infrastructure
{
	using System;
	using Common;
	using NHibernate;

	/// <summary>
	/// Scopes a unit of work around an NHibernate session
	/// </summary>
	public class UnitOfWork : 
		IUnitOfWork
	{
		public const string SessionKey = "NHibernateSession";

		private volatile bool _disposed;
		private bool _ownSession;
		private ITransaction _transaction;

		public UnitOfWork()
		{
			var session = LocalContext.Current.Retrieve<ISession>(SessionKey);
			if (session == null)
				throw new ApplicationException("There is no session active for the unit of work to use");

			_transaction = session.BeginTransaction();
		}

		public UnitOfWork(ISessionFactory sessionFactory)
		{
			var session = LocalContext.Current.Retrieve(SessionKey, () =>
				{
					ISession s = sessionFactory.OpenSession();

					s.FlushMode = FlushMode.Commit;

					_ownSession = true;

					return s;
				});

			_transaction = session.BeginTransaction();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Complete()
		{
			_transaction.Commit();
		}

		public void Fail()
		{
			_transaction.Rollback();
		}

		public void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_transaction.Dispose();
				_transaction = null;

				if (_ownSession)
				{
					var session = LocalContext.Current.Retrieve<ISession>(SessionKey);

					LocalContext.Current.Remove(SessionKey);

					session.Dispose();
				}
			}
			_disposed = true;
		}

		~UnitOfWork()
		{
			Dispose(false);
		}
	}
}