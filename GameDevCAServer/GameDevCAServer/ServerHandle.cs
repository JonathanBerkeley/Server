using System;
using System.Text.RegularExpressions;
using System.Numerics;
using System.Linq;

namespace GameDevCAServer
{
    class ServerHandle
    {
        public static void WelcomeReceived(int _fromClient, Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();
            string _username = _packet.ReadString();

            /*
            try 
            {
                if (Server.clients.ContainsKey(1))
                {
                    for (int i = 1; i < Server.clients.Count; ++i)
                    {
                        if (Server.clients[i].player.username == _username)
                        {
                            Server.clients[_fromClient].Disconnect();
                            return;
                        }
                    }
                }
            } 
            catch (Exception ex)
            {
                Console.WriteLine($"Welcome received exception: {ex}");
            }
            */

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
                //Basic sanitization
                _message = Regex.Replace(_message, @"[><\\]", "");

                Console.WriteLine($"Client {Server.clients[_fromClient].player.username} sent: {_message}");

                //For server commands
                if (_message.Length > 0 && _message[0] == '/')
                {
                    string _command = Regex.Match(_message, @"^(\/[a-z]+)").ToString();

                    if (_command.Length > 0)
                    {
                        string[] serverCommands = { "/msg" };

                        //Server command '/msg'
                        if (_command == serverCommands[0])
                        {
                            try
                            {
                                string[] _args = _message.Split(' ');
                                string _target = _args[1];

                                _message = _message.Remove(0, serverCommands[0].Length + _target.Length + 2);
                                //_message = $"<color=\"#A0A0A0\">whispered: " + _message + "</color>";

                                foreach (var p in Server.clients)
                                {
                                    if (p.Value.player.username == _target)
                                    {
                                        ServerSend.ClientChat(_fromClient, _message, p.Key);
                                        break;
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"{serverCommands[0]} caught {ex.GetType()}");
                            }

                        }
                    }
                }
                else
                {
                    ServerSend.ClientChat(_fromClient, _message);
                }

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
