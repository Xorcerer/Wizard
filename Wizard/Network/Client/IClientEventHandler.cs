using System;
using Xorcerer.Wizard.Network;

namespace Xorcerer.Wizard.Network
{
    public interface IClientEventHandler
    {
        void HandleMessage(IClient client, object message);
        void HandleDisconnected(IClient client, Exception exception);
    }
}

