namespace Stact.Specs.Channels
{
	using Stact.Channels;
	using Stact.Extensions;
	using TestFramework;


	[Scenario]
	public class Sending_a_request
	{
		UntypedChannel _client;
		UntypedChannel _server;

		[Given]
		public void Two_channels()
		{
			_client = new ChannelAdapter();
			_server = new ChannelAdapter();
		}

		[Then]
		public void Sending_a_request_to_one_channel()
		{
			_server.Connect(x =>
				{
					x.AddConsumerOf<Request<MyRequest>>()
						.UsingConsumer(request =>
							{
								request.Respond(new MyResponse());
							});
				});

			var response = new FutureChannel<Response<MyResponse>>();

			_client.Connect(x => x.AddChannel(response));

			_server.Request(new MyRequest(), _client);

			response.WaitUntilCompleted(2.Seconds()).ShouldBeTrue("Timeout waiting for response");

			response.Value.BodyType.ShouldEqual(typeof(MyResponse).ToMessageUrn());
		}


		public class MyRequest
		{
		}

		public class MyResponse
		{
		}
	}
}