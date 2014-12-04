using System;

namespace Xorcerer.Wizard.Network
{
    public static class IClientExtensions
    {
        class Binder : IDisposable
        {
            IClient _client;
            IClientEventHandler _handler;

            public Binder(IClient client, IClientEventHandler handler)
            {
                _client = client;
                _handler = handler;

                client.MessageReceived += _handler.HandleMessage;
                client.Disconnected += _handler.HandleDisconnected;
            }

            #region IDisposable implementation

            void IDisposable.Dispose()
            {
                _client.MessageReceived -= _handler.HandleMessage;
                _client.Disconnected -= _handler.HandleDisconnected;
            }

            #endregion
        }

        public static IDisposable BindEventHandler(this IClient client, IClientEventHandler handler)
        {
            return new Binder(client, handler);
        }
    }
}

