namespace GameDevCAServer
{
    class Constants
    {
        public const int MAX_PLAYERS = 8;
        public const int SERVER_PORT = 24745;
        public const int TICKS_PER_SEC = 75;
        public const int MS_PER_TICK = 1000 / TICKS_PER_SEC;
        public const string SERVER_VERSION = "1.0.8";
        public static readonly string[] ACCEPTED_CLIENT_VERSIONS = {
            "1.3.8", "1.4.0", "1.4.1", "1.4.5"
        };
    }
}
