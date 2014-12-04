using System;
using System.Text;


namespace Xorcerer.Wizard.Network
{
    public class PlainStringSerializer : IMessageSerializer
    {
        #region IMessageSerializer implementation

        public int TryDeserialize(ArraySegment<byte> segment, out object message)
        {
            message = Encoding.UTF8.GetString(segment.Array, segment.Offset, segment.Count);
            return segment.Count;
        }

        public int Serialize(ArraySegment<byte> segment, object message)
        {
            // Warning: Demo Only, not checking the message length here.
            byte[] bytes = Encoding.UTF8.GetBytes((string)message);
            bytes.CopyTo(segment.Array, segment.Offset);
            return bytes.Length;
        }

        #endregion
    }
}

