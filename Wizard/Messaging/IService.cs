using System;
using Castle.Core.Logging;

namespace Xorcerer.Wizard.Messaging
{
    public interface IService : Castle.Core.IStartable
    {
        ILogger Logger { get; }
    }
}

