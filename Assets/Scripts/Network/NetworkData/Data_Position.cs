﻿using MessagePack;

 namespace Network.NetworkData
{
    [MessagePackObject]
    public class Data_Position : Data_Base
    {
        [Key(1)] public int Id { get; set; }
        
        [Key(2)] public float X { get; set; }

        [Key(3)] public float Y { get; set; }

        [Key(4)] public float Z { get; set; }

        public Data_Position()
        {
            Command = NetworkData.Command.Position;
        }
    }
}