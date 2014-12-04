using System;
using Camp.HogRider;
using NUnit.Framework;

namespace HogRiderTest
{
    [TestFixture()]
    public class MessageDispatcherTest
    {
        public class MockMessage
        {
            public int IntField { get; set; }
        }

		public class MockMessage2
		{
			public int IntField { get; set; }
		}

        public class MockHandlerHost
        {
            public MockMessage LastMessage { get; private set; }

            [MessageHandler]
            public void HandleMessage(MockMessage msg)
            {
                LastMessage = msg;
            }

			[MessageHandler]
			public void HandleMessage(MockMessage2 msg)
			{
				//LastMessage = msg;
			}

		}

        [Test()]
        public void HandlerHostTestCase()
        {
            var dispatcher = new MessageDispatcher();

            var host = new MockHandlerHost();
            dispatcher.RegisterHandlerHost(host);

            int exceptedValue = 1;
            dispatcher.Push(new MockMessage { IntField = exceptedValue });
            dispatcher.Dispatch();

            Assert.AreEqual(host.LastMessage.IntField, exceptedValue);
        }

        [Test()]
        public void HandlerTestCase()
        {
            var dispatcher = new MessageDispatcher();

            var host = new MockHandlerHost();
            dispatcher.RegisterHandler<MockMessage>(host.HandleMessage);

            int exceptedValue = 1;
            dispatcher.Push(new MockMessage { IntField = exceptedValue });
            dispatcher.Dispatch();

			dispatcher.UnregisteredMessageReceived += Console.WriteLine;

            Assert.AreEqual(host.LastMessage.IntField, exceptedValue);
        }
    }
}

