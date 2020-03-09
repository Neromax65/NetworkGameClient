using MessagePack;

namespace Network.NetworkData
{
    [MessagePackObject]
    public class Data_Ping : Data_Base
    {
        public Data_Ping()
        {
            Command = NetworkData.Command.Ping;
        }
    }
}