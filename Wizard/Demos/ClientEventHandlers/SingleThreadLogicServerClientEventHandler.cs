using System;
using System.Threading;
using System.Collections.Concurrent;
using Xorcerer.Wizard.Network;


namespace Xorcerer.Wizard.Messaging.Demos
{
    public class SingleThreadLogicServerClientEventHandler : IClientEventHandler
    {
        private Castle.Core.Logging.ILogger _logger = Castle.Core.Logging.NullLogger.Instance;

        public Castle.Core.Logging.ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        ConcurrentQueue<string> _queue = new ConcurrentQueue<string>();
        Thread _thread;

        CancellationToken _cancellationToken;

        public SingleThreadLogicServerClientEventHandler(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _thread = new Thread(() => {
                while (!_cancellationToken.IsCancellationRequested)
                {
                    string message;
                    while (!_queue.TryDequeue(out message))
                    {
                        Thread.Sleep(100);
                        if (_cancellationToken.IsCancellationRequested)
                            return;
                    }
                    Logger.InfoFormat("Received string '{0}', thread id: {1}", message.Trim(),
                                      Thread.CurrentThread.ManagedThreadId);
                }
            });
            _thread.Start();
        }

        #region IClientEventHandler implementation

        public void HandleMessage(Xorcerer.Wizard.Network.IClient client, object message)
        {
            _queue.Enqueue((string)message);
        }

        public void HandleDisconnected(Xorcerer.Wizard.Network.IClient client, Exception exception)
        {
            Logger.Info("Bye bye");
        }

        #endregion
    }
}

