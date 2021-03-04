using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace GameDevCAServer
{
    class ServerHandle
    {
        public static void WelcomeReceived(int _fromClient, Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();
            string _username = _packet.ReadString();

            Console.WriteLine($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}. With username {_username}.");
            if (_fromClient != _clientIdCheck)
            {
                Console.WriteLine($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
            }
            Server.clients[_fromClient].SendIntoGame(_username);
        }


        //This will handle the trusted client data and skip computation of player input on the server
        public static void ReceivePlayerData(int _fromClient, Packet _packet)
        {
            Vector3 _playerLocation = _packet.ReadVector3();
            Quaternion _playerRotation = _packet.ReadQuaternion();

            //Trusting the data and sending it on to other clients
            ServerSend.PlayerLocation(_fromClient, _playerLocation);
            ServerSend.PlayerRotation(_fromClient, _playerRotation);
        }

        public static void ReceiveProjectileData(int _fromClient, Packet _packet)
        {
            Vector3 _projectileLocation = _packet.ReadVector3();
            Quaternion _projectileRotation = _packet.ReadQuaternion();

            ServerSend.ProjectileData(_fromClient, _projectileLocation, _projectileRotation);
        }

        public static void ReceiveClientChat(int _fromClient, Packet _packet)
        {
            String _message = _packet.ReadString();
            if (_message.Length < 100)
            {
                ServerSend.ClientChat(_fromClient, _message);
                Console.WriteLine($"Client {Server.clients[_fromClient].player.username} sent: {_message}");
            }
            else
            {
                Console.WriteLine($"Client {Server.clients[_fromClient].player.username} sent too large of a message!");
            }
        }

        /* For testing UDP
        public static void UDPTestReceived (int _fromClient, Packet _packet)
        {
            string _msg = _packet.ReadString();
            Console.WriteLine($"Received packet through UDP with message: {_msg}");
        }
        */
    }
}
