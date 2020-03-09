﻿using MessagePack;
 
 namespace Network.NetworkData
{
    [MessagePackObject]
    public struct Command
    {
        public const byte None = 0;
        public const byte Ping = 1;
        public const byte Position = 2;
        public const byte Rotation = 3;
        public const byte Scale = 4;
        public const byte Spawn = 5;
        public const byte Connect = 6;
        public const byte Disconnect = 7;
        public const byte Register = 8;
        public const byte Unregister = 9;
    }
}