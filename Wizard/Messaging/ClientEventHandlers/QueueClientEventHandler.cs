/// <summary>
/// Process events with a queue in a standalone task.
/// </summary>
using System;
using System.Collections.Concurrent;
using Xorcerer.Wizard.Network;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace Xorcerer.Wizard.Messaging
{
    public class QueueClientEventHandler<T> : IClientEventHandler, IDisposable
        where T: class
    {
        class Event
        {
            public IClient Client { get; set; }

            /// <summary>
            /// Gets or sets the message.
            /// </summary>
            /// <value>The message, null for disconnection event.</value>
            public T Message { get; set; }

            public Exception Exception { get; set; }
        }

        private Castle.Core.Logging.ILogger _logger = Castle.Core.Logging.NullLogger.Instance;

        public Castle.Core.Logging.ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        const int StateIdle = 0;
        const int StateWorking = 1;
        const int StateDisposed = 2;
        int _workerState = StateIdle;
        Task _worker;
        CancellationTokenSource _cancellationTokenSource;
        readonly ConcurrentQueue<Event> _eventObjectsQueue = new ConcurrentQueue<Event>();
        readonly IMessageAsyncHandler<T> _messageHandler;

		// TODO: canceltoken from outside.
        public QueueClientEventHandler(IMessageAsyncHandler<T> messageHandler)
        {
            _messageHandler = messageHandler;
        }

        public Task WorkerTask
        {
            get
            {
                return _worker;
            }
        }

        void LogExitingThenDispose(Task t)
        {
            _cancellationTokenSource = null;

            Logger.InfoFormat("Worker task exited, status: {0}, exception: {1}", t.Status, t.Exception);

            Dispose();
        }

        Task HandleEventAsync(Event e)
        {
            try
            {
                if (e.Message == null)
                    return _messageHandler.HandleDisconnectionAsync(e.Client, e.Exception);
                return _messageHandler.HandleMessageAsync(e.Client, e.Message);
            }
            catch (Exception ex)
            {
                Logger.Warn("Exception thrown in async messagehandler.", ex);
                e.Client.Close();
                throw;
            }
        }

        #region IClientEventHandler implementation
        /// <summary>
        /// Handles the message event.
        /// </summary>
        /// <param name="client">Client.</param>
        /// <param name="message">Message.</param>
        /// <exception cref="InvalidCastException">When the message is not type of T.</exception>
        public void HandleMessage(IClient client, object message)
        {
            if (client == null)
                throw new ArgumentNullException("client");

            if (message == null)
                throw new ArgumentNullException("message");

            if (_workerState == StateDisposed)
                throw new ObjectDisposedException(GetType().Name);

			_eventObjectsQueue.Enqueue( new Event { Client = client, Message = (T)message });
            
            if (_workerState == StateWorking)
                return;

            if (Interlocked.CompareExchange(ref _workerState, StateWorking, StateIdle) != StateIdle)
                return;

            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = _cancellationTokenSource.Token;

            _worker = Task.Run(async () => {
                Logger.DebugFormat("Starting async event processing loop in <{0}> {1}.", GetType(), GetHashCode());
                while (true)
                {
                    Event e;
                    while (!_eventObjectsQueue.TryDequeue(out e))
                    {
                        token.ThrowIfCancellationRequested();
                        await Task.Yield();
                    }

                    await HandleEventAsync(e);
                }
			}, token);

            _worker.ContinueWith(LogExitingThenDispose);
        }

        public void HandleDisconnected(IClient client, Exception exception)
        {
            if (client == null)
                throw new ArgumentNullException("client");

            if (_workerState == StateDisposed)
                throw new ObjectDisposedException(GetType().Name);

            _eventObjectsQueue.Enqueue(new Event { Client = client, Exception = exception});
        }
        #endregion
        #region IDisposable implementation
        public void Dispose()
        {
            Thread.VolatileWrite(ref _workerState, StateDisposed);

            if (_cancellationTokenSource != null)
                _cancellationTokenSource.Cancel();

            if (_worker != null)
                _worker.Wait();
        }
        #endregion
    }
}

