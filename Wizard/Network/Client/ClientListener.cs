using System;
using System.Net;
using System.Net.Sockets;
using Xorcerer.Wizard.Utilities;

namespace Xorcerer.Wizard.Network
{
    public class ClientListener
    {
        readonly IClientFactory _clientFactory;
        readonly IPEndPoint _endpoint;
        TcpListener _listener;

        volatile ListenerState _state = ListenerState.Idle;

        public IPEndPoint EndPoint { get { return _endpoint; } }
        public ClientListener(IClientFactory clientFactory, IPEndPoint endpoint)
        {
            if (clientFactory == null)
                throw new ArgumentNullException("container");

            if (endpoint == null)
                throw new ArgumentNullException("endpoint");

            _clientFactory = clientFactory;
            _endpoint = endpoint;
        }

        public async void Start()
        {
            if (_state != ListenerState.Idle)
                throw new InvalidOperationException("Already listening.");
            _state = ListenerState.Listerning;

            _listener = new TcpListener(_endpoint);
            _listener.Start();

            while(_state == ListenerState.Listerning)
            {
                TcpClient tcpClient;
                try
                {
                    tcpClient = await _listener.AcceptTcpClientAsync();
                }
                catch (ObjectDisposedException)
                {
                    // The underlying socket has been disposed.
                    break;
                }
                IClient client = _clientFactory.Create(tcpClient);
                if (ClientConnected != null) ClientConnected(client);
            }
        }

        public void Stop()
        {
            _listener.Stop();
            _state = ListenerState.Idle;
        }

        public event Action<IClient> ClientConnected;
    }
}

