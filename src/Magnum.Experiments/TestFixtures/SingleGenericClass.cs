namespace Magnum.Experiments.TestFixtures
{
	public class SingleGenericClass<T> :
		SingleGenericInterface<T>
	{
		private readonly T _key;

		public SingleGenericClass(T key)
		{
			_key = key;
		}

		public T Key
		{
			get { return _key; }
		}

		public void GenericMethod(T obj)
		{
		}
	}
}