using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
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
        
        private Socket _serverConnection;

        private int _pingFailureCount;
        private ILogger _logger;

        // public event Action<INetworkData> DataReceived;
        public event Action<INetworkData> DataReceived;

        public void Connect(string ip, int port)
        {
            _logger = Debug.unityLogger;
            
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            _serverConnection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                
            _logger.Log($"Trying to connect to {ip}:{port}...");
            try
            {
                _serverConnection.Connect(ipEndPoint);
            }
            catch (SocketException ex)
            {
                _logger.LogError("Connection Error", ex.Message);
                return;
            }
            _logger.Log($"Successfully connected to {ip}:{port}");
            NetworkIdGenerator.SetLastId(0);
        }

        public async Task ClientLoop()
        {
            try
            {
                if (_serverConnection == null || !_serverConnection.Connected)
                {
                    CloseConnection();
                    return;
                }
                await ReceiveDataAsync();
            }
            catch (SocketException ex)
            {
                _logger.LogError("Network Error", ex.Message);
                CloseConnection();
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception", ex);
                throw;
            }
        }

        public void SendData(INetworkData data)
        {
            byte[] serializedData = SerializeData(data);
            _serverConnection.Send(serializedData);
        }
        
        public async Task SendDataAsync(INetworkData data)
        {
            byte[] serializedData = SerializeData(data);
            await _serverConnection.SendAsync(new ArraySegment<byte>(serializedData), SocketFlags.None);
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

        private bool IsAnyDataReceived()
        {
            if (_serverConnection.Available > 0) return true;
            if (_pingFailureCount >= Constants.MAX_PING_FAILURE_COUNT)
            {
                _logger.Log($"Disconnecting from server due to not ping for {Constants.MAX_PING_FAILURE_COUNT} ticks.");
                _serverConnection.Disconnect(false);
            }
            return false;
        }
        
        private async Task ReceiveDataAsync()
        {
            if (IsAnyDataReceived())
                _pingFailureCount = 0;
            else
                return;
                
            byte[] buffer = new byte[Constants.BUFFER_SIZE];
            var bytes = await _serverConnection.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
            List<INetworkData> dataList = DeserializeData(buffer, bytes);

            foreach (var data in dataList)
            {
                HandleCommand(data.Command, data);
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
            if (_serverConnection == null || !_serverConnection.Connected)
                return;
            _logger.Log("Disconnecting from server...");
            NetworkIdGenerator.SetLastId(0);
            SendData(new Data_Disconnect());
            _serverConnection.Shutdown(SocketShutdown.Both);
            _serverConnection.Close();
            _serverConnection = null;
        }
        
    }
}
