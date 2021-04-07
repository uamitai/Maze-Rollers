using System.Net;

public static class ClientSend
{
    //send a packet
    private static void TcpSendData(Packet packet)
    {
        packet.WriteLength();
        Client.tcp.SendData(packet);
    }

    //response to welcome packet, send ID and public IP
    public static void WelcomeResponse()
    {
        Packet packet = new Packet((int)ClientPackets.welcomeResponse);
        packet.Write(Client.clientID);

        //get public IP
        packet.Write(new WebClient().DownloadString("http://icanhazip.com").Trim());

        TcpSendData(packet);

        packet.Dispose();
    }

    //send code of opened room
    public static void OpenRoom()
    {
        Packet packet = new Packet((int)ClientPackets.openRoom);
        TcpSendData(packet);
        packet.Dispose();
    }

    //send request for IP of room with ID
    public static void RequestRoomID(string roomID)
    {
        Packet packet = new Packet((int)ClientPackets.requestHostIP);
        packet.Write(roomID);
        TcpSendData(packet);
        packet.Dispose();
    }

    //tell server to close room
    public static void CloseRoom(string roomID)
    {
        Packet packet = new Packet((int)ClientPackets.closeRoom);
        packet.Write(roomID);
        TcpSendData(packet);
        packet.Dispose();
    }

    //request access to account
    public static void LoginUser(string username, string password)
    {
        Packet packet = new Packet((int)ClientPackets.loginUser);
        packet.Write(username);
        packet.Write(password);
        TcpSendData(packet);
        packet.Dispose();
    }

    //create new account
    public static void RegisterUser(string username, string password)
    {
        Packet packet = new Packet((int)ClientPackets.registerUser);
        packet.Write(username);
        packet.Write(password);
        TcpSendData(packet);
        packet.Dispose();
    }

    //send colours to store at database
    public static void SetColours(int colour1, int colour2)
    {
        Packet packet = new Packet((int)ClientPackets.setColours);
        packet.Write(colour1);
        packet.Write(colour2);
        TcpSendData(packet);
        packet.Dispose();
    }
}
