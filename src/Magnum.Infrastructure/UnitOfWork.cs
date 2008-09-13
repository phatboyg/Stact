namespace Magnum.Infrastructure
{
    using System;
    using NHibernate;
    using Rhino.Commons;

    public class UnitOfWork :
        IDisposable
    {
        private readonly ITransaction _trx;

        public UnitOfWork()
        {
            _trx = ((ISession) Local.Data["session"]).BeginTransaction();
        }


        public void Complete()
        {
            _trx.Commit();
        }

        public void Fail()
        {
            _trx.Rollback();
        }

        public void Dispose()
        {
            _trx.Dispose();
        }

        public static UnitOfWork Begin()
        {
            return new UnitOfWork();
        }
    }
}