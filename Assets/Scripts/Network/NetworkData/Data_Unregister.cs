﻿using MessagePack;

namespace Network.NetworkData
{
    /// <summary>
    /// Network unregister data class
    /// </summary>
    [MessagePackObject]
    public class Data_Unregister : Data_Base
    {
        /// <summary>
        /// Network identity of object
        /// </summary>
        [Key(1)] public int Id { get; set; }

        public Data_Unregister()
        {
            Command = NetworkData.Command.Unregister;
        }
    }
}