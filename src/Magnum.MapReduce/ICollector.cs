namespace Magnum.MapReduce
{
	public interface ICollector<K, V>
	{
		void Collect(K key, V value);
	}
}