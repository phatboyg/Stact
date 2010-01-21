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
	using System.Data;
	using Infrastructure.Data;
	using NHibernate;
	using NUnit.Framework;
	using Rhino.Mocks;
	using TestFramework;

	[TestFixture]
	public class NHibernateUnitOfWork_Specs
	{
		[Test]
		public void Begin_Transaction_Should_Start_A_New_Transaction_With_Default_IsolationLevel()
		{
			var mockSession = MockRepository.GenerateMock<ISession>();
			var mockTransaction = MockRepository.GenerateStub<ITransaction>();

			mockSession.Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted))
				.Return(mockTransaction);

			var unitOfWork = new NHibernateUnitOfWork(mockSession);
			unitOfWork.IsInTransaction.ShouldBeFalse();

			unitOfWork.BeginTransaction();
			unitOfWork.IsInTransaction.ShouldBeTrue();

			mockSession.VerifyAllExpectations();
		}

		[Test]
		public void BeginTransaction_Should_Start_A_New_Transaction_With_Specified_IsolatinLevel()
		{
			var mockSession = MockRepository.GenerateMock<ISession>();
			var mockTransaction = MockRepository.GenerateStub<ITransaction>();

			mockSession.Expect(x => x.BeginTransaction(IsolationLevel.Snapshot))
				.Return(mockTransaction);

			var unitOfWork = new NHibernateUnitOfWork(mockSession);
			unitOfWork.IsInTransaction.ShouldBeFalse();
			unitOfWork.BeginTransaction(IsolationLevel.Snapshot);

			unitOfWork.IsInTransaction.ShouldBeTrue();
			mockSession.VerifyAllExpectations();
		}

		[Test]
		public void BeginTransaction_Throws_InvalidOperationException_When_Transaction_Already_Running()
		{
			var mockSession = MockRepository.GenerateStub<ISession>();
			mockSession.Stub(x => x.BeginTransaction(IsolationLevel.Unspecified))
				.IgnoreArguments().Return(MockRepository.GenerateStub<ITransaction>());

			var unitOfWork = new NHibernateUnitOfWork(mockSession);
			unitOfWork.BeginTransaction();

			unitOfWork.IsInTransaction.ShouldBeTrue();

			Assert.Throws<InvalidOperationException>(() => { unitOfWork.BeginTransaction(); });
		}

		[Test]
		public void Comitting_Transaction_Releases_Transaction_From_UnitOfWork()
		{
			var mockSession = MockRepository.GenerateMock<ISession>();
			mockSession.Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted))
				.Return(MockRepository.GenerateStub<ITransaction>());

			var unitOfWork = new NHibernateUnitOfWork(mockSession);
			Magnum.Data.ITransaction transaction = unitOfWork.BeginTransaction();

			unitOfWork.IsInTransaction.ShouldBeTrue();
			transaction.Commit();
			unitOfWork.IsInTransaction.ShouldBeFalse();
			mockSession.VerifyAllExpectations();
		}

		[Test]
		public void Ctor_Throws_ArgumentNullException_When_ISession_Parameter_Is_Null()
		{
			Assert.Throws<ArgumentNullException>(() => { new NHibernateUnitOfWork(null); });
		}

		[Test]
		public void Dispose_UnitOfWork_Disposed_Underlying_Transaction_And_Session()
		{
			var mockSession = MockRepository.GenerateMock<ISession>();
			var mockTransaction = MockRepository.GenerateMock<ITransaction>();
			mockSession.Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted))
				.Return(mockTransaction);
			mockTransaction.Expect(x => x.Dispose());
			mockSession.Expect(x => x.Dispose());

			using (var unitOfWork = new NHibernateUnitOfWork(mockSession))
			{
				unitOfWork.BeginTransaction();
			}
			mockSession.VerifyAllExpectations();
			mockTransaction.VerifyAllExpectations();
		}

		[Test]
		public void Flush_Calls_Underlying_ISession_Flush()
		{
			var mockSession = MockRepository.GenerateMock<ISession>();
			mockSession.Expect(x => x.Flush());

			var unitOfWork = new NHibernateUnitOfWork(mockSession);
			unitOfWork.Flush();

			mockSession.VerifyAllExpectations();
		}

		[Test]
		public void IsInTransaction_Should_Return_False_When_No_Transaction_Exists()
		{
			var mockSession = MockRepository.GenerateStub<ISession>();
			var unitOfWork = new NHibernateUnitOfWork(mockSession);

			unitOfWork.IsInTransaction.ShouldBeFalse();
		}

		[Test]
		public void Rollback_Transaction_Releases_Transaction_From_UnitOfWork()
		{
			var mockSession = MockRepository.GenerateMock<ISession>();
			mockSession.Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted))
				.Return(MockRepository.GenerateStub<ITransaction>());

			var unitOfWork = new NHibernateUnitOfWork(mockSession);
			Magnum.Data.ITransaction transaction = unitOfWork.BeginTransaction();

			unitOfWork.IsInTransaction.ShouldBeTrue();
			transaction.Rollback();
			unitOfWork.IsInTransaction.ShouldBeFalse();
			mockSession.VerifyAllExpectations();
		}

		[Test]
		public void TransactionalFlush_Rollsback_Transaction_When_Flush_Throws_Exception()
		{
			var mockSession = MockRepository.GenerateMock<ISession>();
			var mockTransaction = MockRepository.GenerateMock<ITransaction>();


			mockSession.Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted))
				.Return(mockTransaction);
			mockSession.Expect(x => x.Flush()).Throw(new Exception());

			mockTransaction.Expect(x => x.Rollback());

			var unitOfWork = new NHibernateUnitOfWork(mockSession);
			Assert.Throws<Exception>(() => { unitOfWork.Commit(); });

			mockSession.VerifyAllExpectations();
			mockTransaction.VerifyAllExpectations();
		}

		[Test]
		public void TransactionalFlush_Starts_A_Transaction_With_Default_Isolation_And_Commits_When_Flush_Succeeds()
		{
			var mockSession = MockRepository.GenerateMock<ISession>();
			var mockTransaction = MockRepository.GenerateMock<ITransaction>();

			mockSession.Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted))
				.Return(mockTransaction);
			mockSession.Expect(x => x.Flush());

			mockTransaction.Expect(x => x.Commit());

			var unitOfWork = new NHibernateUnitOfWork(mockSession);
			unitOfWork.Commit();

			mockSession.VerifyAllExpectations();
			mockTransaction.VerifyAllExpectations();
		}

		[Test]
		public void TransactionalFlush_Starts_A_Transaction_With_Specified_IsolationLevel_And_Commits_When_Flush_Succeeds()
		{
			var mockSession = MockRepository.GenerateMock<ISession>();
			var mockTransaction = MockRepository.GenerateMock<ITransaction>();

			mockSession.Expect(x => x.BeginTransaction(IsolationLevel.ReadUncommitted))
				.Return(mockTransaction);
			mockSession.Expect(x => x.Flush());

			mockTransaction.Expect(x => x.Commit());

			var unitOfWork = new NHibernateUnitOfWork(mockSession);
			unitOfWork.Commit(IsolationLevel.ReadUncommitted);

			mockSession.VerifyAllExpectations();
			mockTransaction.VerifyAllExpectations();
		}

		[Test]
		public void TransactionalFlush_Uses_Existing_Transaction_When_Transactional_AlreadyRunning()
		{
			var mockSession = MockRepository.GenerateMock<ISession>();
			var mockTransaction = MockRepository.GenerateMock<ITransaction>();

			mockSession.Expect(x => x.BeginTransaction(IsolationLevel.ReadCommitted))
				.Return(mockTransaction)
				.Repeat.Once(); //Expect BeginTransaction to be called only once.

			var unitOfWork = new NHibernateUnitOfWork(mockSession);
			unitOfWork.BeginTransaction();
			unitOfWork.Commit();

			mockSession.VerifyAllExpectations();
			mockTransaction.VerifyAllExpectations();
		}
	}
}