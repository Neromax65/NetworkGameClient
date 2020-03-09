using MessagePack;

namespace Network.NetworkData
{
    [MessagePackObject]
    public class Data_Unregister : Data_Base
    {
        public int Id { get; set; }

        public Data_Unregister()
        {
            Command = NetworkData.Command.Unregister;
        }
    }
}