using System;
using System.Threading;

namespace MockClient
{
    class MainClass
    {
        static CommunicationController controller = new CommunicationController();

        public static void Main(string[] args)
        {
            controller.Start();
            while (true)
            {
                controller.FixedUpdate();
                Thread.Sleep(1);
            }
        }
    }
}
