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
	using System.Data;
	using Magnum.Data;
	using NHibernate;
	using ITransaction=Magnum.Data.ITransaction;

	public class NHibernateUnitOfWork :
		IUnitOfWork
	{
		private static Func<ISession> _sessionProvider;
		private volatile bool _disposed;
		private ISession _session;
		private NHibernateTransaction _transaction;

		public NHibernateUnitOfWork(ISession session)
		{
			if (session == null)
				throw new ArgumentNullException("session", "The session must not be null");

			_session = session;
		}

		public ISession Session
		{
			get { return _session; }
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public bool IsInTransaction
		{
			get { return _transaction != null; }
		}

		public ITransaction BeginTransaction()
		{
			return BeginTransaction(IsolationLevel.ReadCommitted); //Default isolation is ReadCommitted
		}

		public ITransaction BeginTransaction(IsolationLevel isolationLevel)
		{
			if (_transaction != null)
				throw new InvalidOperationException("A transaction is already in progress and a new one cannot be started.");

			_transaction = new NHibernateTransaction(_session.BeginTransaction(isolationLevel),
				committed => DisposeTransaction(),
				aborted => DisposeTransaction());

			return _transaction;
		}

		public void Flush()
		{
			_session.Flush();
		}

		public void Commit()
		{
			Commit(IsolationLevel.ReadCommitted);
		}

		public void Commit(IsolationLevel isolationLevel)
		{
			if (!IsInTransaction)
				BeginTransaction(isolationLevel);

			try
			{
				_session.Flush();
				_transaction.Commit();
			}
			catch
			{
				_transaction.Rollback();
				throw;
			}
		}

		public void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				DisposeTransaction();
				DisposeSession();
			}
			_disposed = true;
		}

		private void DisposeSession()
		{
			if (_session == null) return;

			_session.Dispose();
			_session = null;
		}

		private void DisposeTransaction()
		{
			if (_transaction == null) return;

			_transaction.Dispose();
			_transaction = null;
		}

		~NHibernateUnitOfWork()
		{
			Dispose(false);
		}

		public static IUnitOfWork Create()
		{
			if (_sessionProvider == null)
				throw new InvalidOperationException("No session provider has been set");

			return new NHibernateUnitOfWork(_sessionProvider());
		}

		public static NHibernateUnitOfWork Current
		{
			get
			{
				if (!UnitOfWork.IsActive)
					throw new InvalidOperationException("No unit of work has been started, start one before creating a repository");

				NHibernateUnitOfWork current = UnitOfWork.Current as NHibernateUnitOfWork;
				if (current == null)
					throw new InvalidOperationException("A current UnitOfWork must be established with the NHibernateUnitOfWork provider");

				return current;
			}
		}

		public static void SetSessionProvider(Func<ISession> sessionProvider)
		{
			_sessionProvider = sessionProvider;
		}
	}
}