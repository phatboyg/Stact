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
namespace Magnum.Data
{
	using System;
	using System.Data;

	/// <summary>
	/// An interface to resolve from the container to encapsulate a unit of work
	/// </summary>
	public interface IUnitOfWork :
		IDisposable
	{
		/// <summary>
		/// True if the <see cref="UnitOfWork"/> is currently inside of a transaction
		/// </summary>
		bool IsInTransaction { get; }

		/// <summary>
		/// Begins a transaction for the data source
		/// </summary>
		/// <returns>The transaction instance that was created</returns>
		ITransaction BeginTransaction();

		/// <summary>
		/// Begins a transaction for the data source
		/// </summary>
		/// <param name="isolationLevel">The isolation level to use for the transaction</param>
		/// <returns>The transaction instance that was created</returns>
		ITransaction BeginTransaction(IsolationLevel isolationLevel);

		/// <summary>
		/// Flush any changes made outside of a transaction
		/// </summary>
		void Flush();

		/// <summary>
		/// Commit any changes made inside of a transaction
		/// </summary>
		void Commit();

		/// <summary>
		/// Commit any changes made inside of a transaction using the isolation level specified
		/// </summary>
		/// <param name="isolationLevel"></param>
		void Commit(IsolationLevel isolationLevel);
	}
}