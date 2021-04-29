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

        //move to title scene
        SceneManager.LoadScene(offlineScene);
    }

    public override void OnRoomClientAddPlayerFailed()
    {
        base.OnRoomClientAddPlayerFailed();

        //display message accordingly
        MainMenu.singleton.SetErrorTextMenu("Error: Failed to connect to room");
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
