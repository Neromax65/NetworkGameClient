﻿using MessagePack;
 
namespace Network.NetworkData
{
    /// <summary>
    /// Base class to store NetworkData
    /// </summary>
    [Union(0, typeof(Data_Ping))]
    [Union(1, typeof(Data_Position))]
    [MessagePackObject]
    public abstract class Data_Base : INetworkData
    {
        /// <summary>
        /// First byte of Network Data represents command it executes
        /// </summary>
        [Key(0)]
        public byte Command { get; set; }
    }
}