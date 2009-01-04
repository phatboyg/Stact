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
	using Common.Data;

	public class NHibernateTransaction :
		ITransaction
	{
		private readonly Action<NHibernateTransaction> _committed;
		private readonly Action<NHibernateTransaction> _rolledBack;
		private volatile bool _disposed;
		private NHibernate.ITransaction _transaction;

		public NHibernateTransaction(NHibernate.ITransaction transaction, Action<NHibernateTransaction> committed, Action<NHibernateTransaction> rolledBack)
		{
			if (transaction == null)
				throw new ArgumentNullException("transaction", "The transaction must not be null");

			_committed = committed;
			_rolledBack = rolledBack;

			_transaction = transaction;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Commit()
		{
			if (_disposed) throw new ObjectDisposedException("NHibernateTransaction", "The object has been disposed");

			_transaction.Commit();
			_committed(this);
		}

		public void Rollback()
		{
			if (_disposed) throw new ObjectDisposedException("NHibernateTransaction", "The object has been disposed");

			_transaction.Rollback();
			_rolledBack(this);
		}

		public void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				if (_transaction != null)
				{
					_transaction.Dispose();
					_transaction = null;
				}
			}
			_disposed = true;
		}

		~NHibernateTransaction()
		{
			Dispose(false);
		}
	}
}