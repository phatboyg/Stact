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
namespace Magnum.Infrastructure.Specs.Data
{
	using System;
	using Infrastructure.Data;
	using Magnum.Specs;
	using MbUnit.Framework;
	using NHibernate;
	using Rhino.Mocks;

	[TestFixture]
	public class NHibernateTransaction_Specs
	{
		[Test]
		public void Ctor_Throws_ArgumentNullException_When_ITransation_Parameter_Is_Null()
		{
			Assert.Throws<ArgumentNullException>(() => new NHibernateTransaction(null, x => { }, y => { }));
		}

		[Test]
		public void Commit_Calls_Commit_On_Underlying_ITransaction()
		{
			var mockTransaction = MockRepository.GenerateMock<ITransaction>();
			mockTransaction.Expect(x => x.Commit()).IgnoreArguments();

			var transaction = new NHibernateTransaction(mockTransaction, x => { }, y => { });
			transaction.Commit();

			mockTransaction.VerifyAllExpectations();
		}

		[Test]
		public void Rollback_Calls_Rollback_On_Underlying_ITransaction()
		{
			var mockTransaction = MockRepository.GenerateMock<ITransaction>();
			mockTransaction.Expect(x => x.Rollback()).IgnoreArguments();

			var transaction = new NHibernateTransaction(mockTransaction, x => { }, y => { });
			transaction.Rollback();

			mockTransaction.VerifyAllExpectations();
		}

		[Test]
		public void Commit_Raises_TransactionComitted_Event()
		{
			var mockTransaction = MockRepository.GenerateMock<ITransaction>();
			mockTransaction.Expect(x => x.Commit());

			bool commitCalled = false;
			bool rollbackCalled = false;
			var transaction = new NHibernateTransaction(mockTransaction, committed => { commitCalled = true; }, rolledBack => { rollbackCalled = true; });

			transaction.Commit();

			mockTransaction.VerifyAllExpectations();
			commitCalled.ShouldBeTrue();
			rollbackCalled.ShouldBeFalse();
		}

		[Test]
		public void Rollback_Raises_RollbackComitted_Event()
		{
			var mockTransaction = MockRepository.GenerateMock<ITransaction>();
			mockTransaction.Expect(x => x.Rollback());

			bool commitCalled = false;
			bool rollbackCalled = false;
			var transaction = new NHibernateTransaction(mockTransaction, committed => { commitCalled = true; }, rolledBack => { rollbackCalled = true; });

			transaction.Rollback();

			mockTransaction.VerifyAllExpectations();

			commitCalled.ShouldBeFalse();
			rollbackCalled.ShouldBeTrue();
		}
	}
}