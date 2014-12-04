using System;
using Castle.MicroKernel.Registration;
using Xorcerer.Wizard.Network;
using Castle.MicroKernel.Releasers;
using Castle.Facilities.TypedFactory;
using Castle.DynamicProxy;
using Xorcerer.Wizard.Utilities;
using Xorcerer.Wizard.Messaging.ComponentLifeCycleManagement;
using System.Net;
using Castle.Facilities.Startable;

namespace Xorcerer.Wizard.Messaging.Installers
{
    public class DefaultInstaller : IWindsorInstaller
    {
        public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            container.Kernel.ReleasePolicy = new ForgetableLifecycledComponentsReleasePolicy(container.Kernel);

            container.Kernel.AddFacility<TypedFactoryFacility>();
            //container.AddFacility<StartableFacility>(f => f.DeferredStart());

            container.Register(
                Component.For<IInterceptor>().ImplementedBy<CallTraceInterceptor>().Named(CallTraceInterceptor.Key),

				Component.For<IClient>().ImplementedBy<Client>().LifeStyle.Custom<InstantiateAndForgetIt>().IsFallback(),
                Component.For<ClientListener>().LifeStyle.Transient,
                Component.For<IClientFactory>().AsFactory().IsFallback(),
                Component.For<IClientEventHandlerFactory>().AsFactory().IsFallback(),
                Component.For<IService>().ImplementedBy<MessageProcessService>().IsFallback());
        }
    }
}

