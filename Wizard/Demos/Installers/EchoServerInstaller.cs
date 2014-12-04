using System;
using Castle.MicroKernel.Registration;
using Xorcerer.Wizard.Network;
using Castle.DynamicProxy;
using Xorcerer.Wizard.Utilities;
using Castle.Core;
using Castle.MicroKernel.Releasers;
using System.Net;


namespace Xorcerer.Wizard.Messaging.Demos
{
    public class EchoServerInstaller : IWindsorInstaller
    {
        #region IWindsorInstaller implementation

        public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            container.Register(
                Component.For<IPEndPoint>().Instance(new IPEndPoint(IPAddress.Any, 11080)).Named("port-for-clients"),
                Component.For<IPEndPoint>().Instance(new IPEndPoint(IPAddress.Any, 11081)).Named("port-for-servers"),

                Component.For<IMessageSerializer>().ImplementedBy<PlainStringSerializer>(),
                Component.For<IClientEventHandler>().ImplementedBy<EchoServerClientEventHandler>()
				.Interceptors(InterceptorReference.ForKey(CallTraceInterceptor.Key)).Anywhere);
        }

        #endregion
    }
}

