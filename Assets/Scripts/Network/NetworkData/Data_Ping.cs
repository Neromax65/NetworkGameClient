﻿using MessagePack;

namespace Network.NetworkData
{
    /// <summary>
    /// Ping data class
    /// </summary>
    [MessagePackObject]
    public class Data_Ping : Data_Base
    {
        public Data_Ping()
        {
            Command = NetworkData.Command.Ping;
        }
    }
}