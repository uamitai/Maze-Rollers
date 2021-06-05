//the NetworkManager's task is to handle the networking, RoomManager is the derived variant we'll be using in this project
//the RoomManager in addition takes a specified scene and turns it into a lobby, where the server is awaiting connection from the clients
//spawns RoomPlayer prefabs for every connected client in the lobby, and Player prefabs later in the main game
//this script stores the room settings and overrides certain methods called on disconnection


using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class RoomManager : NetworkRoomManager
{
    public static new RoomManager singleton;

    [Header("Player Settings")]
    public string roomID;
    public Mode gameMode;
    public int gameTime;
    public float catchDistance;
    public float staminaRate;
    public float mouseSensitivity;

    public override void Awake()
    {
        base.Awake();
        if(singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        //free mouse cursor
        Cursor.lockState = CursorLockMode.None;

        //move to disconnected scene
        SceneManager.LoadScene(offlineScene);
    }

    //called when trying to connect to a full room
    public override void OnRoomClientAddPlayerFailed()
    {
        base.OnRoomClientAddPlayerFailed();

        if(MainMenu.singleton != null)
        {
            MainMenu.singleton.SetErrorTextMenu("Error: Failed to connect to room");
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);

        if(Game.singleton != null)
        {
            Game.singleton.OnPlayerDisconnect(conn);
        }
    }
}
