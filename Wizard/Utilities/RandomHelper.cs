using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Xorcerer.Wizard.Utilities
{
    static public class ThreadSafeRandom
    {
        static private readonly object s_Lock = new object();
        static private readonly RNGCryptoServiceProvider s_SeedGenerator = new RNGCryptoServiceProvider();
        [ThreadStatic]
        static private Random s_Rand;
        static public Random Rand
        {
            get
            {
                if (s_Rand == null)
                {
                    lock (s_Lock)
                    {
                        if (s_Rand == null)
                        {
                            byte[] RandomNumber = new byte[4];
                            s_SeedGenerator.GetBytes(RandomNumber);
                            int RN = BitConverter.ToInt32(RandomNumber, 0);
                            s_Rand = new Random(RN);
                        }
                    }
                }
                return s_Rand;
            }
        }
    }
}
