namespace Magnum.MapReduce
{
	public interface IMapper<KInput, VInput, KOutput, VOutput>
	{
		void Map(KInput key, VInput value, ICollector<KOutput, VOutput> collector);
	}
}