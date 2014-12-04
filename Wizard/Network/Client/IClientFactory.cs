using System;
using System.Net.Sockets;

namespace Xorcerer.Wizard.Network
{
    public interface IClientFactory
    {
        IClient Create(TcpClient tcpClient);
        void Release(IClient client);
    }
}

