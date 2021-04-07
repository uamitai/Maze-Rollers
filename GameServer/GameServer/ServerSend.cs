using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    class ServerSend
    {
        private static void TcpSendData(int clientID, Packet packet)
        {
            packet.WriteLength();
            Server.clients[clientID].tcp.SendData(packet);
        }

        //welcome packet
        public static void Welcome(int cliendID, string msg)
        {
            //create
            Packet packet = new Packet((int)ServerPackets.welcome);
            packet.Write(cliendID);
            packet.Write(msg);

            //send
            TcpSendData(cliendID, packet);

            //dispose
            packet.Dispose();
        }

        //send player his room code
        public static void GetRoomID(int clientID, string roomID)
        {
            Packet packet = new Packet((int)ServerPackets.getRoomID);
            packet.Write(roomID);
            TcpSendData(clientID, packet);
            packet.Dispose();
        }

        //send IP of host
        public static void GetHostIP(int clientID, string hostIP)
        {
            Packet packet = new Packet((int)ServerPackets.getHostIP);
            packet.Write(hostIP);
            TcpSendData(clientID, packet);
            packet.Dispose();
        }

        //grant access to user account
        public static void LoginResponse(int clientID, bool valid, string error, int colour1, int colour2)
        {
            Packet packet = new Packet((int)ServerPackets.loginResponse);
            packet.Write(valid);
            packet.Write(error);
            packet.Write(colour1);
            packet.Write(colour2);
            TcpSendData(clientID, packet);
            packet.Dispose();
        }

        //response packet to register
        public static void RegisterResponse(int clientID, bool valid, string error)
        {
            Packet packet = new Packet((int)ServerPackets.registerResponse);
            packet.Write(valid);
            packet.Write(error);
            TcpSendData(clientID, packet);
            packet.Dispose();
        }

        public static void ColourResponse(int clientID, bool valid)
        {
            Packet packet = new Packet((int)ServerPackets.colourResponse);
            packet.Write(valid);
            TcpSendData(clientID, packet);
            packet.Dispose();
        }
    }
}
