using System;
using Castle.MicroKernel.Registration;
using Xorcerer.Wizard.Network;
using Castle.MicroKernel.Releasers;

namespace Xorcerer.Wizard.Messaging.Demos
{
    public class SingleThreadLogicServerInstaller : IWindsorInstaller
    {
        #region IWindsorInstaller implementation

        public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            container.Register(
                Component.For<IMessageSerializer>().ImplementedBy<PlainStringSerializer>(),
                Component.For<IClientEventHandler>().ImplementedBy<SingleThreadLogicServerClientEventHandler>());
        }

        #endregion
    }
}

