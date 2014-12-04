using System;
using Newtonsoft.Json;
using System.Text;

namespace Xorcerer.Wizard.Network
{
    public class LengthPrefixStringSerializer : LengthPrefixMessageSerializerBase
    {
        #region LengthPrefixMessageSerializer implementation

        protected override object DoDeserialize(ArraySegment<byte> segment)
        {
            return BitConverter.ToString(segment.Array, segment.Offset, segment.Count);
        }

        protected override int DoSerialize(ArraySegment<byte> segment, object message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes((string)message);
            // Warning: Demo Only, not checking the message length here.
            Array.Copy(bytes, 0, segment.Array, segment.Offset, segment.Count);

            return bytes.Length;
        }

        #endregion
    }
}

