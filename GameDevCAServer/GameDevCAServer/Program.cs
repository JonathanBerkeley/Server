using System;
using System.Threading;
using MongoDB.Driver;
using dotenv.net;

namespace GameDevCAServer
{
    class Program
    {
        private static bool isRunning = false;
        private static IMongoDatabase database;

        static void Main(string[] args)
        {
            Console.Title = "Game Server";
            isRunning = true;
            Thread mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();

            Server.Start(Constants.MAX_PLAYERS, Constants.SERVER_PORT);
        }

        public static IMongoDatabase GetMongoDatabase()
        {
            if (database != null)
                return database;

            var env = DotEnv.Read(options: new DotEnvOptions(probeForEnv: true, probeLevelsToSearch: 4));

            var settings = MongoClientSettings.FromConnectionString(env["DB_URI"]);
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);

            var mongoClient = new MongoClient(settings);
            database = mongoClient.GetDatabase(env["DB_NAME"]);

            return database;
        }


        private static void MainThread()
        {
            DateTime _nextLoop = DateTime.Now;

            while (isRunning)
            {
                while (_nextLoop < DateTime.Now)
                {
                    GameLogic.Update();
                    _nextLoop = _nextLoop.AddMilliseconds(Constants.MS_PER_TICK);

                    //Efficiency code which checks if server skips ahead. Sleeps this thread until catches up with real time
                    //This drastically reduces CPU usage of server
                    if (_nextLoop > DateTime.Now)
                    {
                        Thread.Sleep(_nextLoop - DateTime.Now);
                    }
                }
            }
        }
    }
}
