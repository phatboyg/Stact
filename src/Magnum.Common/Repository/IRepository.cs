namespace Magnum.Common.Repository
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;

	public interface IRepository
	{
		
	}

	public interface IRepository<T> :
		IRepository<T,Guid>
		where T : IAggregateRoot<Guid>
	{
		T Load(Guid id);
	}

	public interface IRepository<T,K> :
		IQueryable<T>,
		IDisposable
		where T: IAggregateRoot<K>
	{
		
	}

	public abstract class RepositoryBase<T,K> : 
		IRepository<T,K>
		where T : IAggregateRoot<K>
	{
		protected abstract IQueryable<T> RepositoryQuery { get; }


		public IEnumerator<T> GetEnumerator()
		{
			return RepositoryQuery.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return RepositoryQuery.GetEnumerator();
		}

		public Expression Expression
		{
			get { return RepositoryQuery.Expression; }
		}

		public Type ElementType
		{
			get { return RepositoryQuery.ElementType; }
		}

		public IQueryProvider Provider
		{
			get { return RepositoryQuery.Provider; }
		}

		public virtual void Dispose()
		{
		}
	}

	public interface IAggregateRoot<TId>
	{
		TId Id { get; }
	}

	public interface IAggregateRoot : 
		IAggregateRoot<Guid>
	{
	}
}