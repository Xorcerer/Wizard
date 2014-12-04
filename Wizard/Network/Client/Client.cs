using System;
using System.Threading.Tasks;
using System.Net.Sockets;
using Xorcerer.Wizard.Utilities;
using System.Runtime.Serialization;
using System.Threading;

namespace Xorcerer.Wizard.Network
{
	public class Client : IClient, IDisposable
	{
        #region Logger
        private Castle.Core.Logging.ILogger _logger = Castle.Core.Logging.NullLogger.Instance;

        public Castle.Core.Logging.ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }
        #endregion Logger

		IMessageSerializer _serializer;
        TcpClient _tcpClient;
        private const int BufferPoolSize = 1024;

		public Client(TcpClient tcpClient, IMessageSerializer serializer)
		{
            if (serializer == null)
                throw new ArgumentNullException("serializer");

            if (tcpClient == null)
                throw new ArgumentNullException("tcpClient");

            _tcpClient = tcpClient;
			_serializer = serializer;
		}

		#region IClient implementation

        public object Tag { get; set; }

		public bool Closed { get; private set; }

        public event Action<IClient, object> MessageReceived;

        /// <summary>
        /// Occurs when on disconnected.
        /// </summary>
        /// <param name="exception">
        /// null for gracefully disconnecting, otherwise the exception causes disconnecting.
        /// <see cref="SocketException">Exception thrown from underlying socket.</see>
        /// <see cref="AggregateException">Exception from OnMessage event callback.</see>
        /// <see cref="SerializationException">Error occurs while deserializing a messages.</see>
        /// </param>
        public event Action<IClient, Exception> Disconnected;

		public Task SendAsync(object message)
		{
            if (Closed)
                throw new ObjectDisposedException(ToString());

            // TODO: Buffer pools?
            var buffer = new byte[BufferPoolSize];
            int packageLength = _serializer.Serialize(new ArraySegment<byte>(buffer, 0, buffer.Length), message);

            Logger.DebugFormat("Sending message: <{0}>, size {1} byte(s).", message.GetType(), packageLength);
            
            return _tcpClient.GetStream().WriteAsync(buffer, 0, packageLength);
		}

        /// <summary>
        /// Keep receiving package and try to deserialize package to message.
        /// Call OnMessage if a completed message received.
        /// Call OnDisconnected if an exception thrown, or Close invoked.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The client closed.</exception>
        public Task Run(CancellationToken cancellationToken)
        {
            if (Closed)
                throw new ObjectDisposedException(ToString());

			return InternalRun(cancellationToken).ContinueWith(t => InternalClose(t.Exception));
        }

		public void Close()
		{
            InternalClose(exception: null);
		}

		#endregion

        async Task InternalRun(CancellationToken cancellationToken)
        {
            var buffer = new byte[BufferPoolSize];
            int lengthBuffered = 0;
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
               
				int lengthReceived = await ReadAsync(buffer, lengthBuffered, cancellationToken);
                if (lengthReceived == 0) break;

                lengthBuffered += lengthReceived;

                int lengthTotalConsumed = 0;
                while (lengthBuffered > 0)
                {
                    object message;
                    int lengthConsumed = TryDeserialize(new ArraySegment<byte>(buffer, lengthTotalConsumed, lengthBuffered), out message);
                    if (lengthConsumed == 0) break;

                    lengthTotalConsumed += lengthConsumed;
                    lengthBuffered -= lengthConsumed;

                    InvokeOnMessage(message);
                }
                
                Array.Copy(buffer, lengthTotalConsumed, buffer, 0, lengthBuffered);
            }
        }

        void InvokeOnMessage(object message)
        {
            if (message == null || MessageReceived == null)
                return;

            Logger.DebugFormat("Message received: <{0}>.", message.GetType());

            try
            {
                MessageReceived(this, message);
            }
            catch (Exception ex)
            {
                throw new AggregateException("Exception thrown from OnMessage event callback(s).", ex);
            }
        }

        Task<int> ReadAsync(byte[] buffer, int lengthBuffered, CancellationToken cancellationToken)
        {
            if (buffer.Length == lengthBuffered)
				throw new BufferOverflowException("Client incoming buffer");

            return _tcpClient.GetStream().ReadAsync(buffer, lengthBuffered, buffer.Length - lengthBuffered, cancellationToken);
        }

        int TryDeserialize(ArraySegment<byte> segment, out object message)
        {
            int lengthReceived = segment.Count;
            int lengthConsumed;
            try
            {
                lengthConsumed = _serializer.TryDeserialize(segment, out message);
            }
            catch (SerializationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializationException(string.Format("The serializer {0} thrown an exception while deserializing.", _serializer), ex);
            }

            if (lengthConsumed > lengthReceived)
                throw new SerializationException(string.Format("The serializer {0} consumed more bytes than provided.", _serializer));
            
			if (lengthConsumed < 0)
                throw new SerializationException(string.Format("The serializer {0} TryDeserialize returned invalidate value.", _serializer));

            return lengthConsumed;
        }

        public void InternalClose(Exception exception)
        {
            if (Closed)
                return;

            Closed = true;

            _tcpClient.Close();

            if (Disconnected != null) Disconnected(this, exception);

            Disconnected = null;
            MessageReceived = null;
        }

        #region IDisposable implementation

        void IDisposable.Dispose()
        {
            Close();
        }

        #endregion
	}
}

