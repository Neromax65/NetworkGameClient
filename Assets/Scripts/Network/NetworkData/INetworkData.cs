using MessagePack;

namespace Network.NetworkData
{
    [Union(0, typeof(Data_Base))]
    [Union(1, typeof(Data_Ping))]
    [Union(2, typeof(Data_Position))]
    [Union(3, typeof(Data_Rotation))]
    [Union(4, typeof(Data_Scale))]
    [Union(5, typeof(Data_Spawn))]
    [Union(6, typeof(Data_Connect))]
    [Union(7, typeof(Data_Disconnect))]
    [Union(8, typeof(Data_Register))]
    [Union(9, typeof(Data_Unregister))]
    public interface INetworkData
    {
        [Key(0)] byte Command { get; set; }
    }
}