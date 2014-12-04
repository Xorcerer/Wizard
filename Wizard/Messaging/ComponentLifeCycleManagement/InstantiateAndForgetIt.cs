using System;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using Castle.Core;

namespace Xorcerer.Wizard.Messaging.ComponentLifeCycleManagement
{
    [Serializable]
    public class InstantiateAndForgetIt : ILifestyleManager
    {
        #region ILifestyleManager implementation

        private IComponentActivator _componentActivator;

        public void Init(IComponentActivator componentActivator, IKernel kernel, ComponentModel model)
        {
            _componentActivator = componentActivator;
        }

        public object Resolve(CreationContext context, IReleasePolicy releasePolicy)
        {
            var burden = context.CreateBurden(_componentActivator, trackedExternally: true);
            return _componentActivator.Create(context, burden);
        }

        public bool Release(object instance)
        {
            return true;
        }

        public void Dispose()
        {

        }

        #endregion

    }
}

