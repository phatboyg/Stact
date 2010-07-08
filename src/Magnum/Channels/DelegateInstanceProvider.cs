namespace Magnum.Channels
{
	using System;
	using Extensions;


	public class DelegateInstanceProvider<TInstance, TChannel> :
		InstanceProvider<TInstance, TChannel>
		where TInstance : class
	{
		readonly Func<TChannel, TInstance> _provider;

		public DelegateInstanceProvider(Func<TChannel, TInstance> provider)
		{
			Guard.AgainstNull(provider);

			_provider = provider;
		}

		public TInstance GetInstance(TChannel message)
		{
			TInstance instance = _provider(message);

			if (instance == null)
			{
				throw new InvalidOperationException(
					"The instance of type {0} was null for the message type {1}".FormatWith(typeof(TInstance).ToShortTypeName(),
					                                                                        typeof(TChannel).ToShortTypeName()));
			}

			return instance;
		}
	}
}