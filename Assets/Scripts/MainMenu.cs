using UnityEngine;
using UnityEngine.UI;

//create button and join input field script
public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuParent;
    [SerializeField] private GameObject colourParent;
    [SerializeField] private InputField join;
    [SerializeField] private Text usernameText;
    [SerializeField] private Text errorTextMenu;
    [SerializeField] private Text errorTextColours;
    [SerializeField] private Image colour1Image;
    [SerializeField] private Image colour2Image;

    public static MainMenu singleton;
    private bool fullScreen = true;
    private int colour1, colour2;

    public static Color[] colours = { Color.blue, Color.cyan, Color.green, Color.yellow, Color.red, Color.magenta, Color.white, Color.gray, Color.black };
    private const string errorMsg = "Error: Can't communicate with server";

    // Start is called before the first frame update
    void Start()
    {
        singleton = this;
        errorTextMenu.text = "";
        errorTextColours.text = "";
        DisplayColours();

        //delete game manager
        if (Game.singleton != null)
        {
            Destroy(Game.singleton.gameObject);
            Game.singleton = null;
        }

        //set username text
        usernameText.text = "Signed in as: " + ClientManager.singleton.username;
    }

    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            fullScreen = !fullScreen;
            Screen.fullScreen = fullScreen;
        }
    }

    #region EnterRoom

    //become host and open a room
    public void CreateRoom()
    {
        if(!Client.isConnected)
        {
            SetErrorTextMenu(errorMsg);
            Client.Connect();
            return;
        }

        //send command to open room
        ClientSend.OpenRoom();
    }

    //send request for room host's IP
    public void JoinRoom()
    {
        if (!Client.isConnected)
        {
            SetErrorTextMenu(errorMsg);
            Client.Connect();
            return;
        }

        //take user input and request host IP
        ClientSend.RequestRoomID(join.text.ToUpper());
        SetErrorTextMenu("");
    }

    public void SetErrorTextMenu(string text)
    {
        singleton.errorTextMenu.text = text;
    }

    #endregion
    #region PickColours

    public void ToggleScreen()
    {
        colourParent.SetActive(menuParent.activeSelf);
        menuParent.SetActive(!menuParent.activeSelf);

        errorTextMenu.text = "";
        errorTextColours.text = "";

        DisplayColours();
    }

    public void Colour1Left()
    {
        colour1 -= 1;
        if(colour1 < 0)
        {
            colour1 = colours.Length - 1;
        }
        colour1Image.color = colours[colour1];
    }

    public void Colour1Right()
    {
        colour1 += 1;
        if (colour1 >= colours.Length)
        {
            colour1 = 0;
        }
        colour1Image.color = colours[colour1];
    }

    public void Colour2Left()
    {
        colour2 -= 1;
        if (colour2 < 0)
        {
            colour2 = colours.Length - 1;
        }
        colour2Image.color = colours[colour2];
    }

    public void Colour2Right()
    {
        colour2 += 1;
        if (colour2 >= colours.Length)
        {
            colour2 = 0;
        }
        colour2Image.color = colours[colour2];
    }

    void DisplayColours()
    {
        colour1 = ClientManager.singleton.colour1;
        colour2 = ClientManager.singleton.colour2;
        colour1Image.color = colours[colour1];
        colour2Image.color = colours[colour2];
    }

    public void ApplyColours()
    {
        ClientManager.singleton.colour1 = colour1;
        ClientManager.singleton.colour2 = colour2;
        ClientSend.SetColours(colour1, colour2);
    }

    public void SetErrorTextColours(string msg)
    {
        errorTextColours.text = msg;
    }

    #endregion

    public void LogOut()
    {
        ClientManager.singleton.LogOut();
    }
}
