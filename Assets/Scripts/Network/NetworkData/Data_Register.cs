using MessagePack;

namespace Network.NetworkData
{
    [MessagePackObject]
    public class Data_Register : Data_Base
    {
        [Key(1)] public int Id { get; set; }
        [Key(2)] public int PrefabIndex { get; set; }
        [Key(3)] public int OwningPlayerId { get; set; }
        
        public Data_Register()
        {
            Command = NetworkData.Command.Register;
        }
    }
}