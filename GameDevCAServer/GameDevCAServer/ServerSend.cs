using System;
using System.Numerics;
using FlagTranslations;

namespace GameDevCAServer
{
    class ServerSend
    {
        private static void SendTCPData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Server.clients[_toClient].tcp.SendData(_packet);
        }

        private static void SendUDPData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Server.clients[_toClient].udp.SendData(_packet);
        }

        private static void SendTCPDataToAll(Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; ++i)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }

        private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; ++i)
            {
                if (i != _exceptClient)
                {
                    Server.clients[i].tcp.SendData(_packet);
                }
            }
        }

        private static void SendUDPDataToAll(Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; ++i)
            {
                Server.clients[i].udp.SendData(_packet);
            }
        }

        private static void SendUDPDataToAll(int _exceptClient, Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; ++i)
            {
                if (i != _exceptClient)
                {
                    Server.clients[i].udp.SendData(_packet);
                }
            }
        }

        private static void SendToAnon(Packet _packet)
        {

        }

        //https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/preprocessor-directives/preprocessor-region
        #region Packets
        public static void Welcome(int _toClient, string _msg, ulong[] _validationData)
        {
            using (Packet _packet = new Packet((int)ServerPackets.welcome))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);
                _packet.Write(_validationData[0]);
                _packet.Write(_validationData[1]);

                SendTCPData(_toClient, _packet);
            }
        }

        /* This is for testing UDP connectivity
        public static void UDPTest(int _toClient)
        {
            using (Packet _packet = new Packet((int)ServerPackets.udpTest))
            {
                _packet.Write("A test packet for UDP.");
                SendUDPData(_toClient, _packet);
            }
        }
        */

        public static void SpawnPlayer(int _toClient, Player _player)
        {
            using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
            {
                _packet.Write(_player.id);
                _packet.Write(_player.username);
                _packet.Write(_player.position);
                _packet.Write(_player.rotation);

                SendTCPData(_toClient, _packet);
            }
        }

        //Tells other clients that a player has disconnected so they can cleanup the prefab and dictionary
        public static void PlayerDisconnected(int _playerID)
        {
            using (Packet _packet = new Packet((int)ServerPackets.playerDisconnected))
            {
                _packet.Write(_playerID);

                SendTCPDataToAll(_packet);
            }
        }

        //My methods for sending client computed data to other clients below
        //Sends the players location to clients
        public static void PlayerLocation(int _playerID, Vector3 _location)
        {
            using (Packet _packet = new Packet((int)ServerPackets.playerPosition))
            {
                _packet.Write(_playerID);
                _packet.Write(_location);

                SendUDPDataToAll(_playerID, _packet);
            }
        }

        //Sends the players rotation to clients
        public static void PlayerRotation(int _playerID, Quaternion _rotation)
        {
            using (Packet _packet = new Packet((int)ServerPackets.playerRotation))
            {
                _packet.Write(_playerID);
                _packet.Write(_rotation);

                SendUDPDataToAll(_playerID, _packet);
            }
        }

        //Custom projectile implementation
        public static void ProjectileData(int _playerID, Vector3 _location, Quaternion _rotation)
        {
            using (Packet _packet = new Packet((int)ServerPackets.projectileData))
            {
                _packet.Write(_playerID);
                _packet.Write(_location);
                _packet.Write(_rotation);

                SendUDPDataToAll(_playerID, _packet);
            }
        }

        //For message receiving and sending between clients
        public static void ClientChat(int _playerID, String _message)
        {
            using (Packet _packet = new Packet((int)ServerPackets.userMessage))
            {
                _packet.Write(_playerID);
                _packet.Write(_message);

                //Message wont render for the player unless it successfully gets through the server
                //Could give latency between sending a message and seeing it on their own screen
                SendTCPDataToAll(_packet);
            }
        }

        //For messaging specific client
        public static void ClientChat(int _playerID, String _message, int _toClient)
        {
            using (Packet _packet = new Packet((int)ServerPackets.userMessage))
            {
                _packet.Write(_playerID);
                _packet.Write(_message);

                SendTCPData(_toClient, _packet);
            }
        }

        //For communicating server messages to the client
        public static void ServerMessage(int _playerID, ServerCodeTranslations _message)
        {
            using (Packet _packet = new Packet((int)ServerPackets.serverControlComms))
            {
                _packet.Write((int)_message);

                SendTCPData(_playerID, _packet);
            }
        }

        #endregion
    }
}
