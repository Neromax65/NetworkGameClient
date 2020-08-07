using UnityEngine;
using MessagePack;

namespace Network.NetworkData
{
    /// <summary>
    /// Scale data class
    /// </summary>
    [MessagePackObject]
    public class Data_Scale : Data_Base
    {
        /// <summary>
        /// Network identity of object
        /// </summary>
        [Key(1)] public int Id { get; set; }
        
        /// <summary>
        /// X, Y, Z scale values represented by Vector3
        /// </summary>
        [Key(2)] public Vector3 Scale { get; set; }
        
        /// <summary>
        /// Flag to force scale object instantly, even when normally it should be interpolated
        /// </summary>
        [Key(3)] public bool Instantly { get; set; }

        public Data_Scale()
        {
            Command = NetworkData.Command.Scale;
        }
    }
}