namespace Stact.Specs.Channels
{
	using System;
	using System.Linq;
	using Magnum.Extensions;
	using Magnum.TestFramework;
	using NUnit.Framework;


	[Scenario]
	[Category("Slow")]
	public class When_a_request_is_sent_via_wcf
	{
		const string _pipeName = "test";
		ChannelConnection _clientConnection;
		Future<Response<TestMessage>> _response;
		ChannelAdapter _client;
		ChannelAdapter _server;
		ChannelConnection _serverConnection;
		Uri _pipeUri;
		WcfChannelHost _host;

		[When]
		public void A_request_is_sent_via_wcf()
		{
			_pipeUri = new Uri("net.pipe://localhost/pipe");

			_response = new Future<Response<TestMessage>>();

			_client = new ChannelAdapter();
			_clientConnection = _client.Connect(x =>
				{
					x.SendToWcfChannel(_pipeUri, _pipeName)
						.HandleOnCallingThread();

					x.AddConsumerOf<Response<TestMessage>>()
						.UsingConsumer(_response.Complete);
				});

			_server = new ChannelAdapter();
			_serverConnection = _server.Connect(x =>
				{
					x.AddConsumerOf<Request<TestMessage>>()
						.UsingConsumer(request => request.Respond(request.Body));
				});

			_host = new WcfChannelHost(new PoolFiber(), _server, _pipeUri, _pipeName);
		}

		[Then]
		public void Should_match_the_defined_channel_input_structure()
		{
			_client.Flatten().Select(c => c.GetType()).ShouldEqual(new[]
				{
					typeof(ChannelAdapter),
					typeof(BroadcastChannel),
					typeof(TypedChannelAdapter<Response<TestMessage>>),
					typeof(ConsumerChannel<Response<TestMessage>>),
					typeof(WcfChannelProxy),
				});
		}

		[Then]
		public void Should_match_the_defined_channel_output_structure()
		{
			_server.Flatten().Select(c => c.GetType()).ShouldEqual(new[]
				{
					typeof(ChannelAdapter),
					typeof(BroadcastChannel),
					typeof(TypedChannelAdapter<Request<TestMessage>>),
					typeof(ConsumerChannel<Request<TestMessage>>)
				});
		}

		[Then, Explicit]
		public void Should_get_the_message()
		{

			// doesn't work yet sadly
			_client.Request(new TestMessage(), _client);

			_response.WaitUntilCompleted(4.Seconds()).ShouldBeTrue();
		}

		[After]
		public void After()
		{
			_host.Dispose();
			_host = null;

			_clientConnection.Dispose();
			_clientConnection = null;
			_client = null;

			_serverConnection.Dispose();
			_serverConnection = null;
			_server = null;
		}


		class TestMessage
		{
		}
	}
}