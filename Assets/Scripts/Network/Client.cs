using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using Network.NetworkData;
using UnityEngine;

namespace Network
{
    public class Client
    {
        public int Id { get; private set; }
        public string PlayerName { get; private set; }

        public Socket ServerConnection { get; private set; }

        private int _pingFailure;
        private ILogger _logger;

        private List<INetworkData> _dataToSend;

        // public event Action<INetworkData> DataReceived;
        public event Action<INetworkData> DataReceived;

        public void Connect(string ip, int port)
        {
            _logger = Debug.unityLogger;
            
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            ServerConnection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                
            _logger.Log($"Trying to connect to {ip}:{port}...");
            try
            {
                ServerConnection.Connect(ipEndPoint);
            }
            catch (SocketException ex)
            {
                _logger.LogError("Connection Error", ex.Message);
                return;
            }
            _dataToSend = new List<INetworkData>();
            _logger.Log($"Successfully connected to {ip}:{port}");
            NetworkIdGenerator.SetLastId(0);
        }

        public async Task ClientLoop()
        {
            try
            {
                SendDataAll();
                await ReceiveDataAsync();
            }
            catch (SocketException ex)
            {
                _logger.LogError("Network Error", ex.Message);
                CloseConnection();
            }
        }

        public void SendDataAdd(INetworkData data)
        {
            _dataToSend.Add(data);
        }

        private void SendData(INetworkData data)
        {
            byte[] serializedData = SerializeData(data);
            ServerConnection.Send(serializedData);
        }

        private void SendDataAll()
        {
            foreach (var data in _dataToSend)
            {
                SendData(data);
            }
            _dataToSend.Clear();
        }
        
        public async Task SendDataAsync(INetworkData data)
        {
            byte[] serializedData = SerializeData(data);
            await ServerConnection.SendAsync(new ArraySegment<byte>(serializedData), SocketFlags.None);
        }

        private byte[] SerializeData(INetworkData data)
        {
            byte[] bytes = MessagePackSerializer.Serialize(data);
            return bytes;
        }
        
        private List<INetworkData> DeserializeData(byte[] serializedData, int totalDataLength)
        {
            try
            {
                List<INetworkData> dataList = new List<INetworkData>();
                int bytesRead = 0;
                // _logger.Log($"Deserializing data of total length: {totalDataLength}");
                do
                {
                    // _logger.Log($"Bytes read: {bytesRead}");
                    INetworkData data = MessagePackSerializer.Deserialize<INetworkData>(serializedData, out var curBytesRead);
                    dataList.Add(data);
                    // _logger.Log($"DataList length: {dataList.Count}");
                    bytesRead += curBytesRead;
                    serializedData = serializedData.Skip(curBytesRead).ToArray();
                    // _logger.Log($"Left to deserialize: {totalDataLength - bytesRead}");
                } while (bytesRead < totalDataLength);
                return dataList;
            }
            catch (Exception ex)
            {
                _logger.LogError("Deserialize Error", ex.Message);
                return null;
            }
        }

        private async Task ReceiveDataAsync()
        {
            if (ServerConnection.Available == 0)
            {
                _pingFailure++;
                if (_pingFailure >= Constants.MAX_PING_FAILURE_COUNT)
                {
                    CloseConnection();
                }
                return;
            }

            _pingFailure = 0;
            byte[] buffer = new byte[Constants.BUFFER_SIZE];
            var bytes = await ServerConnection.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
            List<INetworkData> dataList = DeserializeData(buffer, bytes);

            foreach (var data in dataList)
            {
                HandleCommand(data.Command, data);
            }

            if (ServerConnection != null && ServerConnection.Connected && _dataToSend.Count == 0)
            {
                SendDataAdd(new Data_Ping());
            }
        }
        
        private void HandleCommand(byte command, INetworkData data)
        {
            DataReceived?.Invoke(data);
            switch (command)
            {
                case Command.None:
                    _logger.Log("No command was received.");
                    break;
                case Command.Ping:
                    break;
                case Command.Position:
                    break;
                default:
                    _logger.Log("Unrecognized command.");
                    break;
            }
        }

        public void CloseConnection()
        {
            if (ServerConnection == null || !ServerConnection.Connected)
                return;
            _logger.Log("Disconnecting from server...");
            NetworkIdGenerator.SetLastId(0);
            _dataToSend.Clear();
            _dataToSend.Add(new Data_Disconnect());
            DataReceived += TotallyCloseConnection;
            // SendData(new Data_Disconnect());
            // ServerConnection = null;
            // ServerConnection.Shutdown(SocketShutdown.Both);
            // ServerConnection.Close();
            // ServerConnection = null;
        }

        private void TotallyCloseConnection(INetworkData data)
        {
            if (ServerConnection == null || !ServerConnection.Connected)
                return;
            DataReceived -= TotallyCloseConnection;
            ServerConnection.Shutdown(SocketShutdown.Both);
            ServerConnection.Close();
            ServerConnection = null;
        }
        
    }
}
