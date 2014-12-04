using System;
using System.Threading.Tasks;
using System.Threading;

namespace Xorcerer.Wizard.Network
{
	public interface IClient
	{
		Task SendAsync(object message);

        bool Closed { get; }
		void Close();

        /// <summary>
        /// Keep receiving package and try to deserialize package to message.
        /// Call OnMessage when a completed message received.
        /// Call OnDisconnected when an exception thrown, or Close invoked.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The client closed.</exception>
        Task Run(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Occurs when a complete message received.
        /// Any exception thrown from this method should close the client.
        /// </summary>
        event Action<IClient, object> MessageReceived;

        /// <summary>
        /// Occurs when on disconnected.
        /// </summary>
        /// <param name="exception">
        /// null for gracefully disconnecting, otherwise the exception causes disconnecting.
        /// <see cref="SocketException">Exception thrown from underlying socket.</see>
        /// <see cref="AggregateException">Exception from OnMessage event callback.</see>
        /// <see cref="SerializationException">Error occurs while deserializing a messages.</see>
        /// </param>
        event Action<IClient, Exception> Disconnected;

        /// <summary>
        /// Gets or sets the tag.
        /// The tag is only used by the outer logic, e.g. id, catagorizing, etc.
        /// Inspired by Google Android's view tag.
        /// </summary>
        /// <value>The tag.</value>
        object Tag { get; set; }
	}
}

