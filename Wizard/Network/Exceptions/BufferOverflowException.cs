using System;

namespace Xorcerer.Wizard.Network
{
    public class BufferOverflowException : Exception
    {
        public BufferOverflowException(string bufferName) :
            base(string.Format("Buffer '{0}' overflowed.", bufferName))
        {
        }
    }
}

