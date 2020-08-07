﻿using UnityEngine;
using MessagePack;

namespace Network.NetworkData
{
    /// <summary>
    /// Spawn network object data class
    /// </summary>
    [MessagePackObject]
    public class Data_Spawn : Data_Base
    {
        // TODO: May be send only prefabIndex, Position and Rotation can be send by another package
        /// <summary>
        /// Index of Unity prefab, that represent this object
        /// </summary>
        [Key(1)] public int PrefabIndex { get; set; }
        
        /// <summary>
        /// X, Y, Z coordinates of object represented by Vector3
        /// </summary>
        [Key(2)] public Vector3 Position { get; set; }
        
        /// <summary>
        /// X, Y, Z, W rotation values represented by Quaternion
        /// </summary>
        [Key(3)] public Quaternion Rotation { get; set; }

        public Data_Spawn()
        {
            Command = NetworkData.Command.Spawn;
        }
    }
}