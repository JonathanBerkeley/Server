﻿namespace GameDevCAServer
{
    class Constants
    {
        public const int MAX_PLAYERS = 8;
        public const int SERVER_PORT = 24745;
        public const int TICKS_PER_SEC = 75;
        public const int MS_PER_TICK = 1000 / TICKS_PER_SEC;
        public const string SERVER_VERSION = "1.0.7";
        public static readonly string[] ACCEPTED_CLIENT_VERSIONS = { "1.2.8", "1.3.1", "1.3.2", "1.3.3" };
    }
}
