using MessagePack;

namespace Network.NetworkData
{
    [MessagePackObject]
    public class Data_Spawn : Data_Base
    {
        [Key(1)] public int PrefabIndex { get; set; }
        
        [Key(2)] public float PosX { get; set; }

        [Key(3)] public float PosY { get; set; }

        [Key(4)] public float PosZ { get; set; }
        
        [Key(5)] public float RotX { get; set; }

        [Key(6)] public float RotY { get; set; }

        [Key(7)] public float RotZ { get; set; }
        
        [Key(8)] public float RotW { get; set; }
        

        public Data_Spawn()
        {
            Command = NetworkData.Command.Spawn;
        }
    }
}