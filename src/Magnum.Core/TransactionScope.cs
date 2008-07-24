namespace PhatBoyG.Core
{
	using System;

	public delegate void TransactionScopeMethod();

	public static class TransactionScope
	{
		public static void Do(ITransactionScopeContext scopeContext, TransactionScopeMethod expression)
		{
			scopeContext.BeginTransaction();
			try
			{
				expression();

				scopeContext.CommitTransaction();
			}
			catch (Exception ex)
			{
				scopeContext.RollbackTransaction();

				throw;
			}
		}
	}
}