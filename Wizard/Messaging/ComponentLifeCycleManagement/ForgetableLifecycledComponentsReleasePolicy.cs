using System;
using Castle.MicroKernel;

namespace Xorcerer.Wizard.Messaging.ComponentLifeCycleManagement
{
    [Serializable]
    public class ForgetableLifecycledComponentsReleasePolicy : Castle.MicroKernel.Releasers.LifecycledComponentsReleasePolicy
    {
        public ForgetableLifecycledComponentsReleasePolicy(IKernel kernel) :
            base(kernel)
        {

        }

        readonly Type instantiateAndForgetItType = typeof(InstantiateAndForgetIt);

        public override void Track(object instance, Burden burden)
        {
            if (instantiateAndForgetItType.Equals(burden.Model.CustomLifestyle))
            {
                return;
            }
            base.Track(instance, burden);
        }
    }}

