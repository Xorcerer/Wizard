using System;
using System.Threading;

namespace Xorcerer.Wizard.Network
{
    public interface IClientEventHandlerFactory
    {
        IClientEventHandler Create(CancellationToken cancellationToken);
        void Release(IClientEventHandler handler);
    }
}

