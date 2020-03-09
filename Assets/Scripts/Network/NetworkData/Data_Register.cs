using MessagePack;

namespace Network.NetworkData
{
    [MessagePackObject]
    public class Data_Register : Data_Base
    {
        public int Id { get; set; }
        public int PrefabIndex { get; set; }
        public int OwningPlayerId { get; set; }
        
        public Data_Register()
        {
            Command = NetworkData.Command.Register;
        }
    }
}