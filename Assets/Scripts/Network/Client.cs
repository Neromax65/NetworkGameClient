using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using MessagePack;
using Network.NetworkData;
using UnityEngine;

namespace Network
{
    /// <summary>
    /// Network Client class
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Network identity of a client
        /// </summary>
        public int Id { get; private set; }
        
        /// <summary>
        /// Client`s name
        /// TODO: Currently not in use
        /// </summary>
        public string PlayerName { get; private set; }

        /// <summary>
        /// Socket, that handles the connection between server and client
        /// </summary>
        public Socket ServerConnection { get; private set; }

        /// <summary>
        /// Counter for not receiving any messages from server
        /// </summary>
        private int _pingFailure;
        
        /// <summary>
        /// Custom console logger
        /// </summary>
        private ILogger _logger;

        /// <summary>
        /// Packet, that will be send to the server on next loop
        /// </summary>
        private DataPacket _packet;

        /// <summary>
        /// Event for receiving data from server
        /// </summary>
        public event Action<INetworkData> DataReceived;

        /// <summary>
        /// Connect to the server
        /// </summary>
        /// <param name="ip">IP-address</param>
        /// <param name="port">Port</param>
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
            _packet = new DataPacket();
            _logger.Log($"Successfully connected to {ip}:{port}");
            NetworkIdGenerator.SetLastId(0);
        }

        /// <summary>
        /// Loop, that handles all network data pass
        /// </summary>
        /// <returns></returns>
        public async Task ClientLoop()
        {
            try
            {
                await ReceiveDataAsync();
                SendDataPacket();
            }
            catch (SocketException ex)
            {
                _logger.LogError("Network Error", ex.Message);
                Disconnect();
            }
        }

        /// <summary>
        /// Send current data packet to the server
        /// </summary>
        private void SendDataPacket()
        {
            if (_packet.DataList.Count == 0)
                return;
            var serializedPacket = SerializeDataPacket(_packet);
            ServerConnection.Send(serializedPacket);
            _packet.Clear();
        }

        /// <summary>
        /// Add Network Data to packet
        /// </summary>
        /// <param name="data">Data to add</param>
        public void SendDataAddToPacket(INetworkData data)
        {
            _packet.Add(data);
        }
        
        /// <summary>
        /// Serializing data before sending to the server
        /// </summary>
        /// <param name="packet">Network data packet to serialize</param>
        /// <returns>Array of bytes</returns>
        private byte[] SerializeDataPacket(DataPacket packet)
        {
            byte[] bytes = MessagePackSerializer.Serialize(packet);
            return bytes;
        }
        
        /// <summary>
        /// Deserialize received data
        /// </summary>
        /// <param name="serializedData">Array of bytes to deserialize</param>
        /// <returns>Network DataPacket object</returns>
        private DataPacket DeserializeDataPacket(byte[] serializedData)
        {
            var packet = MessagePackSerializer.Deserialize<DataPacket>(serializedData);
            return packet;
        }

        /// <summary>
        /// Receive network data from server
        /// </summary>
        /// <returns></returns>
        private async Task ReceiveDataAsync()
        {
            if (ServerConnection.Available == 0)
            {
                _pingFailure++;
                if (_pingFailure >= Constants.MAX_PING_FAILURE_COUNT)
                {
                    Disconnect();
                }
                return;
            }

            _pingFailure = 0;
            byte[] buffer = new byte[Constants.BUFFER_SIZE];
            var bytes = await ServerConnection.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
            List<INetworkData> dataList = DeserializeDataPacket(buffer).DataList;

            foreach (var data in dataList)
            {
                HandleCommand(data.Command, data);
            }

            if (ServerConnection != null && ServerConnection.Connected && _packet.DataList.Count == 0)
            {
                _packet.Add(new Data_Ping());
            }
        }
        
        /// <summary>
        /// React to network data from the server based on command
        /// </summary>
        /// <param name="command">Byte that represents command</param>
        /// <param name="data">Actual network data itself</param>
        private void HandleCommand(byte command, INetworkData data)
        {
            DataReceived?.Invoke(data);
            switch (command)
            {
                case Command.None:
                    _logger.Log("No command was received.");
                    break;
                case Command.Ping:
                case Command.Position:
                case Command.Rotation:
                case Command.Spawn:
                case Command.Scale:
                case Command.Connect:
                case Command.Disconnect:
                case Command.Register:
                case Command.Unregister:
                    break;
                default:
                    _logger.Log("Unrecognized command.");
                    break;
            }
        }

        /// <summary>
        /// Disconnect from server
        /// </summary>
        public void Disconnect()
        {
            if (ServerConnection == null || !ServerConnection.Connected)
                return;
            _logger.Log("Disconnecting from server...");
            NetworkIdGenerator.SetLastId(0);
            _packet.Clear();
            _packet.Add(new Data_Disconnect());
            DataReceived += CloseConnection;
            ServerConnection = null;
        }

        /// <summary>
        /// Close socket and shutdown connection
        /// </summary>
        /// <param name="data">Disconection data</param>
        private void CloseConnection(INetworkData data)
        {
            if (ServerConnection == null || !ServerConnection.Connected)
                return;
            DataReceived -= CloseConnection;
            ServerConnection.Shutdown(SocketShutdown.Both);
            ServerConnection.Close();
            ServerConnection = null;
        }
        
    }
}
