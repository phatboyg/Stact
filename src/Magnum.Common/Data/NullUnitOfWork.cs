namespace Magnum.Common.Data
{
    using System;
    using System.Data;

    public class NullUnitOfWork :
        IUnitOfWork
    {
        private static readonly NullUnitOfWork _null = new NullUnitOfWork();

        public static IUnitOfWork Null()
        {
            return _null; 
        }

        public void Dispose()
        {
            //ignore
        }

        public bool IsInTransaction
        {
            get { return false; }
        }

        public ITransaction BeginTransaction()
        {
            return null;
        }

        public ITransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return null;
        }

        public void Flush()
        {
            //ignore
        }

        public void Commit()
        {
            //ignore
        }

        public void Commit(IsolationLevel isolationLevel)
        {
            //ignore
        }
    }
}