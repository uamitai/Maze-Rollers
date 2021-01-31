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

        //create and send a welcome packet
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

        public static void ValidRoomID(int clientID, bool valid)
        {
            Packet packet = new Packet((int)ServerPackets.validRoomID);
            packet.Write(valid);
            TcpSendData(clientID, packet);
            packet.Dispose();
        }

        public static void GetHostIP(int clientID, string hostIP)
        {
            Packet packet = new Packet((int)ServerPackets.getHostIP);
            packet.Write(hostIP);
            TcpSendData(clientID, packet);
            packet.Dispose();
        }

        public static void RoomNotFound(int clientID)
        {
            Packet packet = new Packet((int)ServerPackets.roomNotFound);
            TcpSendData(clientID, packet);
            packet.Dispose();
        }

        public static void ValidLogin(int clientID, bool valid, string error)
        {
            Packet packet = new Packet((int)ServerPackets.validLogin);
            packet.Write(valid);
            packet.Write(error);
            TcpSendData(clientID, packet);
            packet.Dispose();
        }

        public static void ValidRegister(int clientID, bool valid, string error)
        {
            Packet packet = new Packet((int)ServerPackets.validRegister);
            packet.Write(valid);
            packet.Write(error);
            TcpSendData(clientID, packet);
            packet.Dispose();
        }
    }
}
