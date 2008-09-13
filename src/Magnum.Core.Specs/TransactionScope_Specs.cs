namespace Magnum.Core.Tests
{
	using System;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using PhatBoyG.Core;
	using Rhino.Mocks;

	[TestFixture]
	public class When_using_the_TransactionScope_helper_class : Specification
	{
		[Test]
		public void A_transaction_should_be_committed_if_the_expression_succeeds()
		{
			ITransactionScopeContext context = Mock<ITransactionScopeContext>();

			using (Record)
			{
				context.BeginTransaction();
				context.CommitTransaction();
			}

			using (Playback)
			{
				bool called = false;

				TransactionScope.Do(context,
				                    delegate { called = true; });

				Assert.That(called, Is.True);
			}
		}

		[Test]
		public void A_transaction_should_be_rolled_back_if_an_exception_is_thrown()
		{
			ITransactionScopeContext context = Mock<ITransactionScopeContext>();

			using (Record)
			{
				context.BeginTransaction();
				context.RollbackTransaction();
			}

			using (Playback)
			{
				bool called = false;
				try
				{
					TransactionScope.Do(context,
					                    delegate { throw new ApplicationException("Boom!"); });
				}
				catch (ApplicationException ex)
				{
					Assert.That(ex.Message, Is.EqualTo("Boom!"));
					called = true;
				}

				Assert.That(called, Is.True);
			}
		}

		[Test]
		public void If_the_BeginTransaction_fails_then_neither_commit_or_rollback_should_be_called()
		{
			ITransactionScopeContext context = Mock<ITransactionScopeContext>();

			using(Record)
			{
				context.BeginTransaction();
				LastCall.Throw(new NullReferenceException());

				context.RollbackTransaction();
				LastCall.Repeat.Never();

				context.CommitTransaction();
				LastCall.Repeat.Never();
			}

			using(Playback)
			{
				try
				{
					TransactionScope.Do(context, delegate { });
				}
				catch (Exception ex)
				{
					Assert.That(ex, Is.TypeOf(typeof(NullReferenceException)));
				}
			}
			
		}
	}
}