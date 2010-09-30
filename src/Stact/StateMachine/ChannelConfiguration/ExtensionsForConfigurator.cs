namespace Stact.Channels
{
	using Configuration;
	using StateMachine.ChannelConfiguration;
	using Magnum.StateMachine;

	public static class ExtensionsForConfigurator
	{
		public static StateMachineConnectionConfigurator<T> AddConsumersFor<T>(this ConnectionConfigurator configurator)
			where T : StateMachine<T>
		{
			var stateMachineConfigurator = new StateMachineConnectionConfiguratorImpl<T>();

			configurator.RegisterChannelConfigurator(stateMachineConfigurator);

			return stateMachineConfigurator;
		}
	}
}