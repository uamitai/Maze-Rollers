using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace GameServer
{
    class Server
    {
        public static int port { get; private set; }

        public static int maxConns { get; private set; }

        public static Dictionary<int, Client> clients = new Dictionary<int, Client>(); //clientID -> client
        public static Dictionary<int, PacketManager> onPacketReceive; //packetID -> command
        public static Dictionary<string, string> activeRooms = new Dictionary<string, string>(); //roomID -> host ip
        public delegate void PacketManager(int clientID, Packet packet);

        private static TcpListener listener;

        //start the server
        public static void Start(int _port, int _maxConns)
        {
            port = _port;
            maxConns = _maxConns;

            //init tcp listener
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            InitData();
            Console.WriteLine("server started");

            //accept incoming connections
            listener.BeginAcceptTcpClient(new AsyncCallback(TcpConnectCallback), null);
        }

        //async callback to handle incoming connections
        private static void TcpConnectCallback(IAsyncResult result)
        {
            //connect client
            TcpClient client = listener.EndAcceptTcpClient(result);

            //continue accepting connections
            listener.BeginAcceptTcpClient(new AsyncCallback(TcpConnectCallback), null);

            //add client to dict
            for (int i = 0; i < maxConns; i++)
            {
                //search for next empty slot
                if (clients[i + 1].tcp.socket == null)
                {
                    clients[i + 1].tcp.Connect(client);
                    return;
                }
            }

            //server is full
            Console.WriteLine("failed to connect: server is full");
        }

        private static void InitData()
        {
            for (int i = 0; i < maxConns; i++)
            {
                //fill clients dict
                clients.Add(i + 1, new Client(i + 1));
            }

            //commands
            onPacketReceive = new Dictionary<int, PacketManager>()
            {
                { (int)ClientPackets.welcomeResponse, ServerReceive.WelcomeReceived },
                { (int)ClientPackets.openRoom, ServerReceive.StartHost },
                { (int)ClientPackets.requestHostIP, ServerReceive.RequestHostIP },
                { (int)ClientPackets.closeRoom, ServerReceive.CloseRoom },
                { (int)ClientPackets.loginUser, ServerReceive.LoginUser },
                { (int)ClientPackets.registerUser, ServerReceive.RegisterUser },
                { (int)ClientPackets.setColours, ServerReceive.SetColours }
            };
        }
    }
}
