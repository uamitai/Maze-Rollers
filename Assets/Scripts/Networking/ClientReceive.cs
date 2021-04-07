using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public static class ClientReceive
{
    public static void Welcome(Packet packet)
    {
        int id = packet.ReadInt();
        string msg = packet.ReadString();

        Client.clientID = id;
        Debug.Log($"{msg}");

        ClientSend.WelcomeResponse();
    }

    public static void GetRoomID(Packet packet)
    {
        RoomManager.singleton.roomID = packet.ReadString();

        //start hosting
        NetworkManager.singleton.StartHost();
        SceneManager.LoadScene(NetworkManager.singleton.onlineScene);

        Debug.Log("Started hosting");
    }

    public static void GetHostIP(Packet packet)
    {
        NetworkManager manager = NetworkManager.singleton;
        string IP = packet.ReadString();

        if(IP == "")
        {
            MainMenu.singleton.SetErrorTextMenu("Error: Room Not Found");
            return;
        }

        //connect to IP
        Debug.Log($"Received host IP: {IP}");
        MainMenu.singleton.SetErrorTextMenu("Connecting to host...");
        manager.networkAddress = IP;
        manager.StartClient();
    }

    public static void LoginResponse(Packet packet)
    {
        bool valid = packet.ReadBool();
        string error = packet.ReadString();
        int colour1 = packet.ReadInt();
        int colour2 = packet.ReadInt();
        LoginMenu.inst.Login_PasswordField.text = "";

        if (valid)
        {
            //server approves
            ClientManager.singleton.LogIn(LoginMenu.inst.playerUsername);
            ClientManager.singleton.colour1 = colour1;
            ClientManager.singleton.colour2 = colour2;
        }
        else
        {
            //server disproves
            LoginMenu.inst.SetLoginErrorText(error);
        }
    }

    public static void RegisterResponse(Packet packet)
    {
        bool valid = packet.ReadBool();
        string error = packet.ReadString();
        LoginMenu.inst.ResetAllUIElements();

        if (valid)
        {
            //server approves
            ClientManager.singleton.LogIn(LoginMenu.inst.playerUsername);
        }
        else
        {
            //server disproves
            LoginMenu.inst.SetRegisterErrorText(error);
        }
    }

    public static void ColourResponse(Packet packet)
    {
        bool valid = packet.ReadBool();

        if(valid)
        {
            MainMenu.singleton.ToggleScreen();
        }
        else
        {
            MainMenu.singleton.SetErrorTextColours("Error: Couldn't complete operation. Please try again.");
        }
    }
}
