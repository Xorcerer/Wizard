using System;
using System.Diagnostics;

namespace Xorcerer.Wizard.Network
{
    public abstract class LengthPrefixMessageSerializerBase : IMessageSerializer
    {
        public const int LengthOfPrefix = sizeof(int);

        protected abstract object DoDeserialize(ArraySegment<byte> segment);
        protected abstract int DoSerialize(ArraySegment<byte> segment, object message);

        #region IMessageSerializer implementation

        public int TryDeserialize(ArraySegment<byte> segment, out object message)
        {
            message = null;

            if (segment.Count < LengthOfPrefix)
                return 0;

            int messageLength = BitConverter.ToInt32(segment.Array, segment.Offset);
            if (segment.Count - LengthOfPrefix < messageLength)
                return 0;

            Debug.WriteLine("Message received: {0} bytes.", messageLength);

            message = DoDeserialize(new ArraySegment<byte>(segment.Array, segment.Offset + LengthOfPrefix, messageLength));
            return messageLength + LengthOfPrefix;
        }

        public int Serialize(ArraySegment<byte> segment, object message)
        {
            int messageLength = DoSerialize(new ArraySegment<byte>(segment.Array,
                                                                   segment.Offset + LengthOfPrefix, segment.Count - LengthOfPrefix),
                                            message);
            BitConverter.GetBytes(messageLength).CopyTo(segment.Array, segment.Offset);
            return messageLength + LengthOfPrefix;
        }

        #endregion
    }
}

