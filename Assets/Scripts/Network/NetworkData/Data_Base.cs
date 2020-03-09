using MessagePack;

namespace Network.NetworkData
{
    [Union(0, typeof(Data_Ping))]
    [Union(1, typeof(Data_Position))]
    [MessagePackObject]
    public abstract class Data_Base : INetworkData
    {
        [Key(0)]
        public byte Command { get; set; }
    }
}