using System;
using System.Numerics;
using System.Text.RegularExpressions;
using FlagTranslations;

namespace GameDevCAServer
{
    class ServerHandle
    {
        public static void WelcomeReceived(int _fromClient, Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();
            string _username = _packet.ReadString();
            string _clientVersion = _packet.ReadString();

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

            Console.WriteLine($"\n\t{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully.\n" +
                $"\tPlayer ID: \t\t{_fromClient} \n" +
                $"\tUsername: \t\t{_username} \n" +
                $"\tClient version: \t{_clientVersion}\n"
            );

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

                        //Check if request was a valid command
                        for (int i = 0; i < serverCommands.Length; ++i)
                        {
                            if (i == (serverCommands.Length - 1) && _command != serverCommands[i])
                            {
                                ServerSend.ServerMessage(_fromClient, ServerCodeTranslations.invalidCommand);
                                return;
                            }
                        }

                        //Server command '/msg'
                        if (_command == serverCommands[0])
                        {
                            try
                            {
                                string[] _args = _message.Split(' ');
                                
                                //Checks if too few arguments
                                if (_args.Length < 2)
                                {
                                    ServerSend.ServerMessage(_fromClient, ServerCodeTranslations.badArguments);
                                    return;
                                }

                                string _target = _args[1];

                                //Removes the command from the message to be sent
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
                            catch (NullReferenceException)
                            {
                                //The user that the client referenced doesn't exist, reply with error so client knows
                                ServerSend.ServerMessage(_fromClient, ServerCodeTranslations.userNotFound);
                                //Console.WriteLine($"{serverCommands[0]} caught {ex.GetType()}");
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                ServerSend.ServerMessage(_fromClient, ServerCodeTranslations.badArguments);
                            }
                            catch (Exception) { }

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
