using System;
using System.Collections.Concurrent;
using Xorcerer.Wizard.Network;
using System.Threading.Tasks;

namespace Xorcerer.Wizard.Messaging
{
    /// <summary>
    /// Message async handler.
    /// All the methods are garenteed to run in the same threading context.
    /// </summary>
	public interface IMessageAsyncHandler<T>
	{
        Task HandleMessageAsync(IClient client, T message);

        /// <summary>
        /// Handles the disconnected event.
        /// </summary>
        /// <returns>Task.</returns>
        /// <param name="client">Client.</param>
        /// <param name="exception">Exception which caused the disconnection, null for graceful disconnection.</param>
        Task HandleDisconnectionAsync(IClient client, Exception exception = null);
	}
}

