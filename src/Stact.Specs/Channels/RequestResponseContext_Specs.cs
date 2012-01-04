namespace Stact.Specs.Channels
{
    using Internal;
    using MessageHeaders;
    using Stact;
    using Magnum.Extensions;
    using Magnum.TestFramework;


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
                    x.AddConsumerOf<Message<MyRequest>>()
                        .UsingConsumer(request =>
                            {
                                request.Respond(new MyResponse());
                            });
                });

            var response = new FutureChannel<Message<MyResponse>>();

            _client.Connect(x => x.AddChannel(response));

            _server.Send<Message<MyRequest>>(new MessageContext<MyRequest>(new MyRequest(),
                                                                           () => new UntypedChannelActorRef(_client)));

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