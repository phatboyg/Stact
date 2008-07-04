namespace Magnum.Transport.Tests
{
	using System;
	using System.Net;
	using System.Threading;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;

	[TestFixture]
	public class As_a_participant_on_the_network
	{
		[Test]
		public void I_want_to_connection_to_a_remote_participant()
		{
			PeerInitiator pi = new PeerInitiator(_connectionTimeout, _receiveTimeout);

			IPEndPoint ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7321);

			ManualResetEvent _connected = new ManualResetEvent(false);
			ManualResetEvent _accepted = new ManualResetEvent(false);

			PeerConnection client = null;
			PeerConnection server = null;

			pi.Connected += delegate(PeerConnection connection)
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

			pi.ConnectFailed += delegate
			                    	{
			                    		_connected.Set();
			                    		Assert.Fail("Failed to connect to the peer");
			                    	};

			pi.Listen(ip);

			pi.Connect(ip);

			Assert.That(_connected.WaitOne(TimeSpan.FromSeconds(5), true), Is.True, "Timeout waiting for connection");
			Assert.That(_accepted.WaitOne(TimeSpan.FromSeconds(5), true), Is.True, "Timeout waiting for remote connection");


			// at this point they are connected

			TestObject to = new TestObject("/meta/handshake", "1.0", "12345");

			Assert.That(client, Is.Not.Null);
			Assert.That(server, Is.Not.Null);
			client.Send(to);

			object obj = server.Receive(TimeSpan.FromSeconds(8));

			Assert.That(obj, Is.Not.Null);

			Assert.That(obj, Is.TypeOf(typeof (TestObject)), "Invalid Type Received");
		}

		[Test]
		public void I_want_to_have_peers()
		{
			Peer p1 = new Peer();

			PeerInitiator pi = new PeerInitiator(_connectionTimeout, _receiveTimeout);
		}

		private readonly TimeSpan _connectionTimeout = TimeSpan.FromSeconds(10);
		private readonly TimeSpan _receiveTimeout = TimeSpan.FromMinutes(10);
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