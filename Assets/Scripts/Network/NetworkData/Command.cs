﻿﻿using MessagePack;

namespace Network.NetworkData
{
    /// <summary>
    /// Sort of "enum" Command struct, that used to recognize network commands
    /// </summary>
    [MessagePackObject]
    public struct Command
    {
        /// <summary>
        /// Empty command
        /// </summary>
        public const byte None = 0;
        /// <summary>
        /// Any data
        /// </summary>
        public const byte Ping = 1;
        /// <summary>
        /// Network object`s position data
        /// </summary>
        public const byte Position = 2;
        /// <summary>
        /// Network object`s rotation data
        /// </summary>
        public const byte Rotation = 3;
        /// <summary>
        /// Network object`s scale data
        /// </summary>
        public const byte Scale = 4;
        /// <summary>
        /// Command used for spawning network object
        /// </summary>
        public const byte Spawn = 5;
        /// <summary>
        /// Connection data
        /// </summary>
        public const byte Connect = 6;
        /// <summary>
        /// Disconnection data
        /// </summary>
        public const byte Disconnect = 7;
        /// <summary>
        /// Register network object on server
        /// </summary>
        public const byte Register = 8;
        /// <summary>
        /// Unregister network object on server
        /// </summary>
        public const byte Unregister = 9;
    }
}