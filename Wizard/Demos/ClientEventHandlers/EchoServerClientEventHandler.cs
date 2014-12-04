using System;
using Xorcerer.Wizard.Network;

namespace Xorcerer.Wizard.Messaging.Demos
{
    public class EchoServerClientEventHandler : IClientEventHandler
    {
        private Castle.Core.Logging.ILogger _logger = Castle.Core.Logging.NullLogger.Instance;

        public Castle.Core.Logging.ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        #region IClientEventHandler implementation

        public void HandleMessage(IClient client, object message)
        {
            client.SendAsync((string)message);
        }

        public void HandleDisconnected(IClient client, Exception exception)
        {
            Logger.Info("Bye bye!");
        }

        #endregion
    }
}

