using System;
using System.Collections.Generic;
using System.Net.Sockets;
using ProtoBuf.Meta;
using System.IO;
using System.Net;

namespace Camp.HogRider
{
    public class Server
    {
        TcpClient _client;
        readonly string _host;
        readonly int _port;
        readonly int _bufferSize;
        int _nBytesAvailable;

        Dictionary<int, Type> _idTypeMap = new Dictionary<int, Type>();
        Dictionary<Type, int> _typeIdMap = new Dictionary<Type, int>();

        public event Action<object> MessageReceived;
        public event Action Connected;
        public event Action Disconnected;
        public event Action<Exception> ExceptionRaised;
		
		public const int DefaultBufferSize = 2 << 11;
		
        public Server(string host, int port, int bufferSize = DefaultBufferSize)
        {
            _host = host;
            _port = port;

            _bufferSize = bufferSize;

            StaticProtocolRegistration.RegisterAll(this);
        }

        public void Register(int typeId, Type type)
        {
            _idTypeMap[typeId] = type;
            _typeIdMap[type] = typeId;
        }

        void OnException(Exception ex)
        {
            Stop();

            if (ExceptionRaised != null)
                ExceptionRaised(ex);
        }

        public static IPAddress GetIPAddress(string host)
        {
            // Dns may be blocked without internet connection even if the host is actually an IP string.
            IPAddress result;
            if (IPAddress.TryParse(host, out result))
                return result;

            IPAddress[] candidates = Dns.GetHostAddresses(host);
            if (candidates.Length == 1)
                return candidates[0];

            var random = new Random();
            int index = random.Next(candidates.Length);
            return candidates[index];
        }

        #region Connecting

        public void Start()
        {
            if (_client != null)
                return;

            IPAddress address = GetIPAddress(_host);

            _client = new TcpClient();
            _client.BeginConnect(address, _port, OnConnected, state: null);
        }

        void OnConnected(IAsyncResult result)
        {
            try
            {
                _client.EndConnect(result);
                if (Connected != null) Connected();
                ReceiveLoop();
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        public void Stop()
        {
            if (_client == null)
                return;

            _client.Close();
            if (Disconnected != null) Disconnected();
            _client = null;

            _nBytesAvailable = 0;
        }

        #endregion

        #region Receiving

        void ReceiveLoop(byte[] buffer = null)
        {
            buffer = buffer ?? new byte[_bufferSize];
            _client.GetStream().BeginRead(buffer, 0, buffer.Length, OnReceived, state: buffer);
        }

        void OnReceived(IAsyncResult result)
        {
            var buffer = (byte[])result.AsyncState;
            try
            {
                int nByteReceived = _client.GetStream().EndRead(result);
                if (nByteReceived == 0)
                {
                    Stop();
                    return;
                }
                _nBytesAvailable += nByteReceived;
                TryDeserialize(buffer);
                ReceiveLoop(buffer);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        const int HeaderLength = sizeof(int) * 2;

        void TryDeserialize(byte[] buffer)
        {
            // |-------------|-------------|------------------....|
            //   totalLength    type id        message
            //               |<---         totalLength        --->|
            if (_nBytesAvailable < HeaderLength)
                return;

            int totalLength = BitConverter.ToInt32(buffer, 0);
            if (totalLength + sizeof(int) > _nBytesAvailable)
                return;

            int typeId = BitConverter.ToInt32(buffer, sizeof(int));

            var messageLength = totalLength - sizeof(int);
            using (var ms = new MemoryStream(buffer, HeaderLength, messageLength, writable: false))
            {
                var message = RuntimeTypeModel.Default.Deserialize(ms, null, _idTypeMap[typeId]);
                if (MessageReceived != null)
                    MessageReceived(message);
            }

            _nBytesAvailable -= totalLength;
            // It is ok for _nBytesAvailable to be ZERO.
            Array.Copy(buffer, totalLength + sizeof(int), buffer, 0, _nBytesAvailable);
        }

        #endregion

        #region Sending

        public void Send(object message)
        {
            byte[] buffer = new byte[_bufferSize];

            // Type id, write it down with first 4 bytes skipped (preserved for messageLength).
            int typeId = _typeIdMap[message.GetType()];
            BitConverter.GetBytes(typeId).CopyTo(buffer, sizeof(int));

            using (var ms = new MemoryStream(buffer, HeaderLength,
                                             buffer.Length - HeaderLength, writable: true))
            {
                RuntimeTypeModel.Default.Serialize(ms, message);
                int messageLength = (int)ms.Position;
                BitConverter.GetBytes(messageLength + sizeof(int)).CopyTo(buffer, 0);
                _client.GetStream().BeginWrite(buffer, 0, messageLength + HeaderLength, OnSent, state: null);
            }
        }

        void OnSent(IAsyncResult result)
        {
            try
            {
                _client.GetStream().EndWrite(result);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        #endregion
    }
}

