namespace PhatBoyG.Core
{
	public interface ITransactionScopeContext
	{
		void BeginTransaction();
		void CommitTransaction();
		void RollbackTransaction();
	}
}