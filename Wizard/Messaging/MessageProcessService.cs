using System;
using Castle.Windsor;
using Xorcerer.Wizard.Network;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using log4net;

namespace Xorcerer.Wizard.Messaging
{
    public class MessageProcessService : IService
    {
        ClientListener _listener;
        IClientEventHandlerFactory _clientEventHandlerFactory;

        CancellationTokenSource _cancellationTokenSource;

        private Castle.Core.Logging.ILogger _logger = Castle.Core.Logging.NullLogger.Instance;

        public Castle.Core.Logging.ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        public MessageProcessService(IClientEventHandlerFactory clientEventHandlerFactory, ClientListener clientListener)
        {
            if (clientEventHandlerFactory == null)
                throw new ArgumentNullException("clientEventHandlerFactory");

            if (clientListener == null)
                throw new ArgumentNullException("clientListener");

            _clientEventHandlerFactory = clientEventHandlerFactory;
            _listener = clientListener;
            _listener.ClientConnected += OnConnected;
        }

        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _listener.Start();

            Logger.InfoFormat("Server started, listening on: {0}", _listener.EndPoint);
        }

        public void Stop()
        {
            Logger.InfoFormat("Server is stopping...");
            _listener.Stop();
            _cancellationTokenSource.Cancel();

            Logger.InfoFormat("Server stopped.");
        }

        void OnConnected(IClient client)
        {
			var handler = _clientEventHandlerFactory.Create(_cancellationTokenSource.Token);
            client.BindEventHandler(handler);
            client.Disconnected += (c, e) => LogTaskExiting(c, e, handler);
            client.Run(_cancellationTokenSource.Token);
        }

        void LogTaskExiting(IClient client, Exception ex, IClientEventHandler handler)
        {
            Logger.InfoFormat("Client with tag '{0}' Exited, exception: {1}.", client.Tag, ex);

            _clientEventHandlerFactory.Release(handler);
        }
    }
}

