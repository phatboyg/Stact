using System.Collections.Generic;

namespace Magnum.Specs.Integration
{
    public interface IClassStorageContext
    {
        void Save<T>(T item);
        T Get<T>(object id) where T : class;
        IList<T> List<T>() where T : class;
    }
}