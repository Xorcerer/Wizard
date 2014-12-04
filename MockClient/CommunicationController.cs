using System;
using Camp.HogRider;
using System.Diagnostics;
using Camp.Protocols.Player;

namespace MockClient
{
    class CommunicationController
    {
        Server _server;
        MessageDispatcher _dispatcher;

        public void Start()
        {
            _server = new Server("localhost", 11080);

            // Hard coded protocol id, should replace it with reading from json config.
            // Hard coded protocol id

            _server.Connected += OnConnected;
            _server.Disconnected += () => Debug.WriteLine("Disconnected.");
            _server.ExceptionRaised += e =>
            {
                Console.WriteLine(e);
                throw e;
            };

            _dispatcher = new MessageDispatcher();
            // TODO: Create Echo message for both server/client.
            _dispatcher.RegisterHandler<Echo>(m => Debug.WriteLine(m.ToString()));

            // Connect the server and dispather.
            _server.MessageReceived += _dispatcher.Push;

            _server.Start();
        }

        void OnConnected()
        {
            Debug.WriteLine("Server connected, try login...");
            // TODO: Create Echo message for both server/client.
            _server.Send(echo);
        }

        public void FixedUpdate()
        {
            _dispatcher.Dispatch();
        }
    }
}

