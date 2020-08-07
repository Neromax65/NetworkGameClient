using UnityEngine;
using MessagePack;

namespace Network.NetworkData
{
    /// <summary>
    /// Rotation data class
    /// </summary>
    [MessagePackObject]
    public class Data_Rotation : Data_Base
    {
        /// <summary>
        /// Network identity of object
        /// </summary>
        [Key(1)] public int Id { get; set; }
        
        /// <summary>
        /// X, Y, Z, W rotation values represented by Quaternion
        /// </summary>
        [Key(2)] public Quaternion Rotation { get; set; }
        
        /// <summary>
        /// Flag to force rotate object instantly, even when normally it should be interpolated
        /// </summary>
        [Key(3)] public bool Instantly { get; set; }

        public Data_Rotation()
        {
            Command = NetworkData.Command.Rotation;
        }
    }
}