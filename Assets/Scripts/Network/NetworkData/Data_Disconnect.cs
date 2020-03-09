using MessagePack;

namespace Network.NetworkData
{
    [MessagePackObject]
    public class Data_Disconnect : Data_Base
    {
        public Data_Disconnect()
        {
            Command = NetworkData.Command.Disconnect;
        }
    }
}