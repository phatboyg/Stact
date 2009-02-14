namespace Magnum.ActorModel
{
	using Channels;

	public interface ChannelFactory
	{
		Channel<T> GetChannel<T>();
	}
}