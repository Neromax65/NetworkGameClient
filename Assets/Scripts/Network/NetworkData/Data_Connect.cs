using MessagePack;

namespace Network.NetworkData
{
    [MessagePackObject]
    public class Data_Connect : Data_Base
    {


        public Data_Connect()
        {
            Command = NetworkData.Command.Connect;
        }
    }
}