namespace Magnum.MapReduce
{
	using System.Collections.Generic;

	public interface IReducer<KInput, VInput, KOutput, VOutput>
	{
		void Reduce(KInput key, IEnumerable<VInput> values, ICollector<KOutput, VOutput> collector);
	}
}