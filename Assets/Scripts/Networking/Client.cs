//class that handles communication with the server
//has methods for connection, sending and receiving data
//reads packets and executes commands using the thread opened on the main program


using System;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;

public static class Client
{
    public static TCP tcp;
    public static int clientID;

    public static string ip = "172.16.16.118"; //change according to server IP
    public static int port = 420;
    public static int dataBufferSize = 4096;

    public static bool isConnected;
    private delegate void PacketManager(Packet packet);
    private static Dictionary<int, PacketManager> onPacketReceive;

    public static void Start()
    {
        tcp = new TCP();
        tcp.Connect();
        InitData();
        isConnected = false;
    }

    private static void InitData()
    {
        onPacketReceive = new Dictionary<int, PacketManager>()
        {
            { (int)ServerPackets.welcome, ClientReceive.Welcome },
            { (int)ServerPackets.getRoomID, ClientReceive.GetRoomID },
            { (int)ServerPackets.getHostIP, ClientReceive.GetHostIP },
            { (int)ServerPackets.loginResponse, ClientReceive.LoginResponse },
            { (int)ServerPackets.registerResponse, ClientReceive.RegisterResponse },
            { (int)ServerPackets.colourResponse, ClientReceive.ColourResponse }
        };
    }

    public static void Connect()
    {
        tcp.Connect();
    }

    public static void Disconnect()
    {
        tcp.Disconnect();
    }

    public class TCP
    {
        public TcpClient socket;

        private NetworkStream stream;
        private byte[] receiveBuffer;
        private Packet packetData;

        public void Connect()
        {
            //create socket
            socket = new TcpClient();
            socket.ReceiveBufferSize = dataBufferSize;
            socket.SendBufferSize = dataBufferSize;

            //look for connection
            receiveBuffer = new byte[dataBufferSize];
            socket.BeginConnect(ip, port, ConnectCallback, null);
        }

        private void ConnectCallback(IAsyncResult result)
        {
            //end connect operation
            socket.EndConnect(result);

            //check if connected
            if (!socket.Connected)
            {
                return;
            }
            isConnected = true;

            //receive data
            stream = socket.GetStream();
            packetData = new Packet();
            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }

        public void SendData(Packet packet)
        {
            try
            {
                if(socket == null)
                {
                    return;
                }

                stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
            }
            catch(Exception ex)
            {
                Debug.Log($"error at send data: {ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                //get stream length
                int length = stream.EndRead(result);
                if (length <= 0)
                {
                    Disconnect();
                    return;
                }

                byte[] data = new byte[length];
                Array.Copy(receiveBuffer, data, length);

                //handle data
                packetData.Reset(HandleData(data));

                //continue receiving
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch
            {
                Disconnect();
            }
        }

        //returns if packet should be reset
        private bool HandleData(byte[] data)
        {
            int length = 0; //packet length
            packetData.SetBytes(data); //byte[] data -> packet class

            if (packetData.UnreadLength() >= 4)
            {
                //first int is packet length
                length = packetData.ReadInt();
                if (length <= 0)
                {//packet is empty
                    return true;
                }
            }

            //iterate over packet
            while (length > 0 && packetData.UnreadLength() <= length)
            {
                byte[] packetBytes = packetData.ReadBytes(length);

                //create a thread
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    Packet packet = new Packet(packetBytes);
                    int packetID = packet.ReadInt(); //get packet id
                    onPacketReceive[packetID](packet); //invoke call for packet

                    packet.Dispose();
                });

                length = 0;
                if (packetData.UnreadLength() >= 4)
                {
                    length = packetData.ReadInt();
                    if (length <= 0)
                    {
                        return true;
                    }
                }
            }

            if(length <= 1)
            {
                return true;
            }

            return false;
        }

        public void Disconnect()
        {
            Debug.Log("Disconnected from server");
            isConnected = false;

            stream = null;
            packetData = null;
            receiveBuffer = null;

            if(socket != null)
            {
                socket.Close();
                socket = null;
            }
        }
    }
}
