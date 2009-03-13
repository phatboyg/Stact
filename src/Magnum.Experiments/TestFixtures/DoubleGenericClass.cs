namespace Magnum.Experiments.TestFixtures
{
	public class DoubleGenericClass<T, V> :
		DoubleGenericInterface<T, V>
	{
		private readonly T _key;
		private readonly V _value;

		public DoubleGenericClass(T key, V value)
		{
			_key = key;
			_value = value;
		}

		public T Key
		{
			get { return _key; }
		}

		public V Value
		{
			get { return _value; }
		}
	}
}