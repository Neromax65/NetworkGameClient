﻿﻿using MessagePack;
using UnityEngine;

namespace Network.NetworkData
{
    /// <summary>
    /// Position data class
    /// </summary>
    [MessagePackObject]
    public class Data_Position : Data_Base
    {
        /// <summary>
        /// Network identity of object
        /// </summary>
        [Key(1)] public int Id { get; set; }
        
        /// <summary>
        /// X, Y, Z coordinates of object represented by Vector3
        /// </summary>
        [Key(2)] public Vector3 Position { get; set; }
        
        /// <summary>
        /// Flag to force move object instantly, even when normally it should be interpolated
        /// </summary>
        [Key(3)] public bool Instantly { get; set; }

        public Data_Position()
        {
            Command = NetworkData.Command.Position;
        }
    }
}