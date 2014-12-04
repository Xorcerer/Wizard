using System;
using Castle.DynamicProxy;

namespace Xorcerer.Wizard.Utilities
{
    public class CallTraceInterceptor : IInterceptor
    {
        public const string Key = "call-trace";

        private Castle.Core.Logging.ILogger _logger = Castle.Core.Logging.NullLogger.Instance;

        public Castle.Core.Logging.ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        #region IInterceptor Members

        public void Intercept(IInvocation invocation)
        {
            var fullName = string.Format("{0}::{1}", invocation.Method.DeclaringType, invocation.Method.Name);

            Logger.DebugFormat("Method '{0}' begin -- ", fullName);
            invocation.Proceed();
            Logger.DebugFormat("Method '{0}' returned {1} -- ", fullName, invocation.ReturnValue);
        }

        #endregion
    }
}

