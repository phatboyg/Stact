namespace Magnum.Experiments.TestFixtures
{
	public interface DoubleGenericInterface<T,V>
	{
		T Key { get; }
		V Value { get; }
	}
}