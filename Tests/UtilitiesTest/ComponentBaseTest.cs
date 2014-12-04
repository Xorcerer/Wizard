using System;
using NUnit.Framework;
using Xorcerer.Wizard.Utilities;

namespace UtilitiesTest
{
    [TestFixture()]
    public class ComponentBaseTest
    {
        public class TesteeMessage
        {
            public bool Touched = false;
        }

        public class TesteeComponent: ComponentBase
        {
            public void OnMessageSpecialization(TesteeMessage m)
            {
                m.Touched = true;
            }
        }

        [Test()]
        public void OnMessageMultiDispatchTestCase()
        {
            var message = new TesteeMessage();
            var testee = new TesteeComponent();

            testee.OnMessage(1);
            testee.OnMessage("string");
            testee.OnMessage(message);

            Assert.True(message.Touched);
        }
    }
}

