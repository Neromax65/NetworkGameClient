using MessagePack;

namespace Network.NetworkData
{
    [MessagePackObject]
    public class Data_Connect : Data_Base
    {

        [Key(1)] public string PlayerName { get; set; }
        public Data_Connect()
        {
            Command = NetworkData.Command.Connect;
        }
    }
}