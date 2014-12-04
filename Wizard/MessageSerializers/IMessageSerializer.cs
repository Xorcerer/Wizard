using System;
using Xorcerer.Wizard.Network;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.IO;

namespace Xorcerer.Wizard.Network
{
    public interface IMessageSerializer
	{
		/// <summary>
		/// Tries the deserialize the buffer.
		/// </summary>
		/// <returns>The length of buffer consumed.</returns>
		/// <param name="segment">buffer.</param>
		/// <param name="message">Message if deserialized.</param>
        int TryDeserialize(ArraySegment<byte> segment, out object message);
        int Serialize(ArraySegment<byte> segment, object message);
    }
}
