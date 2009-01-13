namespace Magnum.Common.Data
{
    public class NullTransaction :
        ITransaction
    {
        private static readonly NullTransaction _null = new NullTransaction();

        public static NullTransaction Null
        {
            get { return _null; }
        }

        public void Dispose()
        {
            //ignore
        }

        public void Commit()
        {
            //ignore
        }

        public void Rollback()
        {
            //ignore
        }
    }
}