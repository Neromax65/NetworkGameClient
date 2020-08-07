﻿using MessagePack;

namespace Network.NetworkData
{
    /// <summary>
    /// Connection data class
    /// </summary>
    [MessagePackObject]
    public class Data_Connect : Data_Base
    {
        /// <summary>
        /// Name that player should receive on connection
        /// </summary>
        [Key(1)] public string PlayerName { get; set; }

        public Data_Connect()
        {
            Command = NetworkData.Command.Connect;
        }
    }
}