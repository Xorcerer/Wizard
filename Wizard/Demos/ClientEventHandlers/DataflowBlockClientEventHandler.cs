using System;
using Xorcerer.Wizard.Network;

namespace Xorcerer.Wizard.Messaging.Demos
{
    public class DataflowBlockClientEventHandler : IClientEventHandler
    {
        public DataflowBlockClientEventHandler()
        {
            throw new NotImplementedException();
            //ActionBlock<int> b = new ActionBlock<int>(x => x + 1);
        }

        #region IClientEventHandler implementation

        public void HandleMessage(IClient client, object message)
        {
            throw new NotImplementedException();
        }

        public void HandleDisconnected(IClient client, Exception exception)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

