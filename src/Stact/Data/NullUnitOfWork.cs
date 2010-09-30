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
namespace Stact.Data
{
	using System.Data;

	public class NullUnitOfWork :
		IUnitOfWork
	{
		private static readonly NullUnitOfWork _null = new NullUnitOfWork();

		private ITransaction _transaction;

		public void Dispose()
		{
			_transaction = null;
		}

		public bool IsInTransaction
		{
			get { return _transaction != null; }
		}

		public ITransaction BeginTransaction()
		{
			_transaction = NullTransaction.Null;

			return _transaction;
		}

		public ITransaction BeginTransaction(IsolationLevel isolationLevel)
		{
			_transaction = NullTransaction.Null;

			return _transaction;
		}

		public void Flush()
		{
		}

		public void Commit()
		{
			_transaction = null;
		}

		public void Commit(IsolationLevel isolationLevel)
		{
			_transaction = null;
		}

		public static IUnitOfWork Null()
		{
			return _null;
		}
	}
}