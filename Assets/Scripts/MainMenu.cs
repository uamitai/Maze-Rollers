using UnityEngine;
using UnityEngine.UI;
using Mirror;

//create button and join input field script
public class MainMenu : MonoBehaviour
{
    [SerializeField] private InputField join;
    [SerializeField] private Text usernameText;
    [SerializeField] private Text errorText;

    public static MainMenu inst;
    private System.Random rand;

    private const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ123456789";
    private const int roomNameLength = 5;
    private const string errorMsg = "Error: Can't communicate with server";

    // Start is called before the first frame update
    void Start()
    {
        inst = this;
        errorText.text = "";

        if(ClientManager.inst.isLoggedIn)
        {
            //set username text
            usernameText.text = "Signed in as: " + ClientManager.inst.playerUsername;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            Application.Quit();
        }
    }

    public void CreateRoom()
    {
        if(!Client.isConnected)
        {
            SetErrorText(errorMsg);
            Client.Connect();
            return;
        }

        //generate and set room ID
        RoomManager.roomID = GetRoomID(roomNameLength);

        //register room at server
        ClientSend.StartHost(RoomManager.roomID);
    }

    public void JoinRoom()
    {
        if (!Client.isConnected)
        {
            SetErrorText(errorMsg);
            Client.Connect();
            return;
        }

        //take user input and request host IP
        ClientSend.RequestRoomID(join.text.ToUpper());
        SetErrorText("");
    }

    public void SetErrorText(string text)
    {
        inst.errorText.text = text;
    }

    public string GetRoomID(int nameLength)
    {
        //return a random string of given length
        rand = new System.Random();
        string roomName = "";

        for (int i = 0; i < nameLength; i++)
        {
            roomName += chars[rand.Next(chars.Length)];
        }

        return roomName;
    }

    public void LogOut()
    {
        ClientManager.inst.LogOut();
    }
}
