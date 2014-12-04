using System;
using System.Diagnostics;

namespace Xorcerer.Wizard.Utilities
{
    public abstract class ComponentBase : IComponent
    {
        private Castle.Core.Logging.ILogger _logger = Castle.Core.Logging.NullLogger.Instance;

        public Castle.Core.Logging.ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        public IComponentContainer Parent  { get; protected set; }

        #region IComponent implementation

        public virtual void OnMessage(object message)
        {
            ((dynamic)this).OnMessageSpecialization((dynamic)message);
        }

        public virtual void AttachTo(IComponentContainer container)
        {
            Debug.Assert(Parent == null);

            Parent = container;
        }

        public virtual void Deattach()
        {
            Parent = null;
        }

        #endregion

        public void OnMessageSpecialization(object message)
        {
            Logger.DebugFormat("Unhandled message '{0}' of type '{1}'.", message, message.GetType());
        }
    }
}

