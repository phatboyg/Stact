namespace Magnum.Experiments.TestFixtures
{
	using System.Diagnostics;

	public class SimpleClass
	{
		public void SingleGenericMethod<T>(T obj)
		{
			
		}

		public void DoubleGenericMethod<T,V>(T obj, V value)
		{
			
		}

		public void DoubleGenericMethod<T,V>(DoubleGenericInterface<T,V> instance)
		{
			Trace.WriteLine("Key = " + instance.Key);
			Trace.WriteLine("Value = " + instance.Value);
			
		}
	}
}