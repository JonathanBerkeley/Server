using System.Numerics;

namespace GameDevCAServer
{

    //Tracks player information
    class Player
    {
        public int id;
        public string username;
        public string hwid;

        public Vector3 position;
        public Quaternion rotation;

        public Player(int _id, string _username, string _hwid, Vector3 _spawnPosition)
        {
            id = _id;
            username = _username;
            hwid = _hwid;
            position = _spawnPosition;
            rotation = Quaternion.Identity;
        }
    }
}
