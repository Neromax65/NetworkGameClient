﻿using MessagePack;

namespace Network.NetworkData
{
    /// <summary>
    /// NetworkObject register data class
    /// </summary>
    [MessagePackObject]
    public class Data_Register : Data_Base
    {
        /// <summary>
        /// Network identity of object
        /// </summary>
        [Key(1)] public int Id { get; set; }
        
        /// <summary>
        /// Index of Unity prefab, that represent this object
        /// </summary>
        [Key(2)] public int PrefabIndex { get; set; }
        
        /// <summary>
        /// Player network identity, that owns this object 
        /// </summary>
        [Key(3)] public int OwningPlayerId { get; set; }
        
        public Data_Register()
        {
            Command = NetworkData.Command.Register;
        }
    }
}