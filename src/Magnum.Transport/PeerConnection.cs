namespace Magnum.Transport.Specs
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using Serialization;

    public class PeerConnection : IConnection
    {
        private readonly BDecode _decoder;
        private readonly Action<PeerConnection> _disposeCallback;
        private readonly EndPoint _endpoint;
        private readonly Socket _socket;
        private readonly NetworkStream _stream;
        private readonly IObjectFormatter _formatter;
        private readonly IObjectSerializer _serializer;

        public PeerConnection(Socket socket, EndPoint endpoint, Action<PeerConnection> disposeCallback)
        {
            _endpoint = endpoint;
            _disposeCallback = disposeCallback;

            _socket = socket;
            _stream = new NetworkStream(_socket, FileAccess.ReadWrite, true);

            _stream.ReadTimeout = 1200000;
            _stream.WriteTimeout = 10000;

            _decoder = new BDecode(_stream);

            _formatter = new BEncodeObjectFormatter(_stream);
            _serializer = new BasicObjectSerializer(_formatter);
        }

        public EndPoint Endpoint
        {
            get { return _endpoint; }
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.SuppressFinalize(this);

                _serializer.Dispose();
                _formatter.Dispose();
                _decoder.Dispose();
                _stream.Dispose();

                // can't dispose a socket for some reason, so this should handle it
                using (_socket)
                    _socket.Shutdown(SocketShutdown.Both);
            }
            else
            {
                Action<PeerConnection> disposeCallback = _disposeCallback;
                if (disposeCallback != null)
                {
                    disposeCallback(this);
                }
            }
        }

        public void Send<T>(T message) where T : class
        {
            //_serializer.Serialize(message);
        }

        public object Receive(TimeSpan timeout)
        {
            object obj = _decoder.Read(timeout);

            return obj;
        }
    }
}