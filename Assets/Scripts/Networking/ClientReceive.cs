using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class ClientReceive
{
    public static void Welcome(Packet packet)
    {
        int id = packet.ReadInt();
        string msg = packet.ReadString();

        Client.id = id;
        Debug.Log($"{msg}");

        ClientSend.WelcomeReceived();
    }

    public static void ValidRoomID(Packet packet)
    {
        if(packet.ReadBool())
        {
            //room ID is valid
            Debug.Log("Started hosting");
            NetworkManager.singleton.StartHost();
            SceneManager.LoadScene(NetworkManager.singleton.onlineScene);
        }
        else
        {
            MainMenu.inst.SetErrorText("Error: Room Code Invalid");
        }
    }

    public static void GetHostIP(Packet packet)
    {
        NetworkManager manager = NetworkManager.singleton;
        string IP = packet.ReadString();

        //connect to IP
        Debug.Log($"Received host IP: {IP}");
        manager.networkAddress = IP;
        manager.StartClient();
    }

    public static void RoomNotFound(Packet packet)
    {
        MainMenu.inst.SetErrorText("Error: Room Not Found");
    }

    public static void ValidLogin(Packet packet)
    {
        bool valid = packet.ReadBool();
        string error = packet.ReadString();
        LoginMenu.inst.Login_PasswordField.text = "";

        if (valid)
        {
            //server approves
            ClientManager.inst.LogIn(LoginMenu.inst.playerUsername, LoginMenu.inst.playerPassword);
        }
        else
        {
            //server disproves
            LoginMenu.inst.SetLoginErrorText(error);
        }
    }

    public static void ValidRegister(Packet packet)
    {
        bool valid = packet.ReadBool();
        string error = packet.ReadString();
        LoginMenu.inst.ResetAllUIElements();

        if (valid)
        {
            //server approves
            ClientManager.inst.LogIn(LoginMenu.inst.playerUsername, LoginMenu.inst.playerPassword);
        }
        else
        {
            //server disproves
            LoginMenu.inst.SetRegisterErrorText(error);
        }
    }
}
