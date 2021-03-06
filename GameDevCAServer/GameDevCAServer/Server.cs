﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using FlagTranslations;

namespace GameDevCAServer
{
    class Server
    {
        public static int MaxPlayers { get; private set; }
        public static int Port { get; private set; }
        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
        public delegate void PacketHandler(int _fromClient, Packet _packet);
        public static Dictionary<int, PacketHandler> packetHandlers;

        private static TcpListener tcpListener;
        private static UdpClient udpListener;
        public static void Start(int _maxPlayers, int _port)
        {
            MaxPlayers = _maxPlayers;
            Port = _port;

            //Server information formatted for the console
            Console.WriteLine(
                $"SERVER INFORMATION:\n" +
                $"_______________________________\n\n" +
                $"Version: \t{Constants.SERVER_VERSION}\n" +
                $"Max players: \t{Constants.MAX_PLAYERS}\n" +
                $"TPS: \t\t{Constants.TICKS_PER_SEC}\n" +
                $"_______________________________\n"
            );

            InitializeServerData();

            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);

            udpListener = new UdpClient(Port);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            Console.WriteLine($"Server started on {Port}.");
        }

        private static void TCPConnectCallback(IAsyncResult _result)
        {
            TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
            tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);
            Console.WriteLine($">Incoming connection from: {_client.Client.RemoteEndPoint}");

            for (int i = 1; i <= MaxPlayers; ++i)
            {
                if (clients[i].tcp.socket == null)
                {
                    clients[i].tcp.Connect(_client);
                    return;
                }
            }

            Console.WriteLine($"{_client.Client.RemoteEndPoint} failed to connect: Server full!");
            clients[-1].tcp.Errored(_client, ServerCodeTranslations.serverFull);
        }

        private static void UDPReceiveCallback(IAsyncResult _result)
        {
            try
            {
                IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] _data = udpListener.EndReceive(_result, ref _clientEndPoint);
                udpListener.BeginReceive(UDPReceiveCallback, null);

                if (_data.Length < 4)
                {
                    return;
                }

                using (Packet _packet = new Packet(_data))
                {
                    int _clientId = _packet.ReadInt();

                    //Client ID should never be 0, attempting 0 would crash server
                    if (_clientId == 0)
                    {
                        return;
                    }

                    if (clients[_clientId].udp.endPoint == null)
                    {
                        clients[_clientId].udp.Connect(_clientEndPoint);
                        return;
                    }

                    //This comparison is done to ensure client has a genuine ID
                    if (clients[_clientId].udp.endPoint.ToString() == _clientEndPoint.ToString())
                    {
                        clients[_clientId].udp.HandleData(_packet);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving UDP data: {ex}");
            }
        }

        public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
        {
            try
            {
                if (_clientEndPoint != null)
                {
                    udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending data to {_clientEndPoint} through UDP: {ex}");
            }
        }

        private static void InitializeServerData()
        {
            for (int i = 1; i <= MaxPlayers; ++i)
            {
                clients.Add(i, new Client(i));
            }

            //Used to reply to error clients
            clients.Add(-1, new Client(-1));

            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived },
                { (int)ClientPackets.playerData, ServerHandle.ReceivePlayerData },
                { (int)ClientPackets.projectileData, ServerHandle.ReceiveProjectileData },
                { (int)ClientPackets.userMessage, ServerHandle.ReceiveClientChat }
                //{ (int)ClientPackets.udpTestReceived, ServerHandle.UDPTestReceived }
            };
        }
    }
}
