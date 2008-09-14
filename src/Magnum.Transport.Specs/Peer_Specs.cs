namespace Magnum.Transport.Specs
{
    using System;
    using System.Net;
    using System.Threading;
    using Machine.Specifications;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [Concern("Peer")]
    public class As_a_participant_on_the_network
    {
        private static PeerInitiator _peerInitiator;
        private static readonly TimeSpan _connectionTimeout = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan _receiveTimeout = TimeSpan.FromMinutes(10);
        static PeerConnection client;
        static PeerConnection server;
        private static object deserializedObject;

        private Establish context = () =>
                                        {
                                            _peerInitiator = new PeerInitiator(_connectionTimeout, _receiveTimeout);
                                            var ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7321);

                                            var _connected = new ManualResetEvent(false);
                                            var _accepted = new ManualResetEvent(false);


                                            _peerInitiator.Connected += delegate(PeerConnection connection)
                                                                {
                                                                    if (connection.Endpoint == ip)
                                                                    {
                                                                        client = connection;
                                                                        _connected.Set();
                                                                    }
                                                                    else
                                                                    {
                                                                        server = connection;
                                                                        _accepted.Set();
                                                                    }
                                                                };

                                            _peerInitiator.ConnectFailed += delegate
                                                                    {
                                                                        _connected.Set();
                                                                        Assert.Fail("Failed to connect to the peer");
                                                                    };

                                            _peerInitiator.Listen(ip);

                                            _peerInitiator.Connect(ip);

                                            Assert.That(_connected.WaitOne(TimeSpan.FromSeconds(5), true), Is.True,
                                                        "Timeout waiting for connection");
                                            Assert.That(_accepted.WaitOne(TimeSpan.FromSeconds(5), true), Is.True,
                                                        "Timeout waiting for remote connection");


                                            // at this point they are connected
                                        };

        private Cleanup after_each = () =>
                                         {
                                             _peerInitiator = null;
                                         };

        private Because of = () =>
                                 {
                                     TestObject to = new TestObject("/meta/handshake", "1.0", "12345");

                                     client.Send(to);

                                     deserializedObject = server.Receive(TimeSpan.FromSeconds(8));

                                 };

        private It should_receive_the_sent_object = () =>
                                                        {
                                                            Assert.That(deserializedObject, Is.Not.Null);
                                                            Assert.That(deserializedObject,
                                                                        Is.TypeOf(typeof(TestObject)),
                                                                        "Invalid Type Received");

                                                        };

        private It should_be_able_to_send_it_back = () =>
                                                        {
                                                            server.Send(deserializedObject);

                                                            object o2 = client.Receive(TimeSpan.FromSeconds(3));

                                                            Assert.That(o2, Is.Not.Null);
                                                            Assert.That(o2, Is.TypeOf(typeof(TestObject)), "Invalid Type Received");
                                                        };



    }

    [Serializable]
    public class TestObject
    {
        private readonly string _channel;
        private readonly string _version;
        private readonly string _key;

        public TestObject(string channel, string version, string key)
        {
            _channel = channel;
            _key = key;
            _version = version;
        }

        public string Channel
        {
            get { return _channel; }
        }

        public string Version
        {
            get { return _version; }
        }

        public string Key
        {
            get { return _key; }
        }
    }

    public class Peer
    {
    }
}