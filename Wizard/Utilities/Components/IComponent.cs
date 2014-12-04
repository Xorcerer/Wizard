using System;

namespace Xorcerer.Wizard.Utilities
{
    public interface IComponent
    {
        void OnMessage(object message);

        void AttachTo(IComponentContainer container);
        void Deattach();
    }
}

