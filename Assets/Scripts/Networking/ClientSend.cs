using UnityEngine;

public class ClientSend
{
    private static void TcpSendData(Packet packet)
    {
        packet.WriteLength();
        Client.tcp.SendData(packet);
    }

    //create and send welcome received packet
    public static void WelcomeReceived()
    {
        Packet packet = new Packet((int)ClientPackets.welcomeReceived);
        packet.Write(Client.id);

        TcpSendData(packet);

        packet.Dispose();
    }

    //create and send start host packet
    public static void StartHost(string roomID)
    {
        Packet packet = new Packet((int)ClientPackets.startHost);
        packet.Write(roomID);
        TcpSendData(packet);
        packet.Dispose();
    }

    //request room id packet
    public static void RequestRoomID(string roomID)
    {
        Packet packet = new Packet((int)ClientPackets.requestHostIP);
        packet.Write(roomID);
        TcpSendData(packet);
        packet.Dispose();
    }

    public static void CloseRoom(string roomID)
    {
        Packet packet = new Packet((int)ClientPackets.closeRoom);
        packet.Write(roomID);
        TcpSendData(packet);
        packet.Dispose();
    }

    public static void LoginUser(string username, string password)
    {
        Packet packet = new Packet((int)ClientPackets.loginUser);
        packet.Write(username);
        packet.Write(password);
        TcpSendData(packet);
        packet.Dispose();
    }

    public static void RegisterUser(string username, string password)
    {
        Packet packet = new Packet((int)ClientPackets.registerUser);
        packet.Write(username);
        packet.Write(password);
        TcpSendData(packet);
        packet.Dispose();
    }
}
