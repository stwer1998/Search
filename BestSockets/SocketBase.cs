﻿using BestSockets.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace BestSockets
{
    public abstract class SocketBase<TReceivedData, TSentData>
    {
        public SocketBase(string ip, int port, IObjectSerializer objectSerializer = null)
        {
            _ip = IPAddress.Parse(ip);
            _port = port;
            
            _serializer = objectSerializer ?? new DefaultObjectSerializer();
        }

        protected void InitializeSocket()
        {
            _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        }

        protected async Task<TReceivedData> ReceiveDataAsync(Socket usedSocket)
        {
            var receivedBytes = await ReceiveAllAvailableBytes(usedSocket);

            var receivedObject = _serializer.Deserialize(receivedBytes.ToArray());
            return (TReceivedData)receivedObject;
        }

        protected async Task SendDataAsync(TSentData data, Socket usedSocket)
        {
            var serializedData = _serializer.Serialize(data);
            await usedSocket.SendAsync(serializedData, SocketFlags.None);
        }

        protected void FinalizeSocket()
        {
            _socket.Close();
        }

        protected readonly IPAddress _ip;
        protected readonly int _port;
        protected Socket _socket;

        private readonly IObjectSerializer _serializer;

        private async Task<List<byte>> ReceiveAllAvailableBytes(Socket usedSocket)
        {
            const int bufferSize = 1024;
            var buffer = new byte[bufferSize];
            var receivedBytes = new List<byte>();

            do
            {
                var countOfReceivedBytes = await usedSocket.ReceiveAsync(buffer, SocketFlags.None);
                receivedBytes.AddRange(buffer.Take(countOfReceivedBytes));
            }
            while (usedSocket.Available > 0);

            return receivedBytes;
        }
    }
}
