namespace Magnum.ActorModel.WebSpecs
{
	using Channels;
	using StructureMap;

	public class StructureMapChannelFactory :
		ChannelFactory
	{
		public Channel<T> GetChannel<T>()
		{
			return ObjectFactory.GetInstance<Channel<T>>();
		}
	}
}