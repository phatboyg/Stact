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
					x.ReceiveFromWcfChannel(_pipeUri, _pipeName);

					x.AddConsumerOf<Request<TestMessage>>()
						.UsingConsumer(request => request.Respond(request.Body));
				});
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

		[Then]
		public void Should_get_the_message()
		{
			_client.Request(new TestMessage(), _client);

			_response.WaitUntilCompleted(2.Seconds()).ShouldBeTrue();
		}

		[After]
		public void After()
		{
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