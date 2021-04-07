using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

//lobby scene script
public class Lobby : NetworkBehaviour
{
    [Scene] [SerializeField] private string gameScene;

    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject lobbyMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject settingsButton;
    [SerializeField] private GameObject playerListItemPrefab;

    [SerializeField] private Text roomName;
    [SerializeField] private Text playerCount;
    [SerializeField] private Transform playerListParent;

    public SyncList<string> players = new SyncList<string>();
    [SyncVar] private string roomID;
    [SyncVar] private int maxConns;

    public static Lobby singleton;

    private NetworkManager manager;
    private List<GameObject> playerList;
    private bool fullScreen = true;
    private bool lobbyActive = true;

    // Start is called before the first frame update
    void Start()
    {
        singleton = this;
        manager = NetworkManager.singleton;
        playerList = new List<GameObject>();

        if(isServer)
        {
            roomID = RoomManager.singleton.roomID;
            maxConns = manager.maxConnections;
        }
        else
        {
            lobbyMenu.SetActive(true);
        }

        //only host can view start button
        startButton.SetActive(isServer);
        settingsButton.SetActive(isServer);
        roomName.text = "ROOM CODE: " + roomID;

        players.Callback += OnlistChange;
    }

    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            fullScreen = !fullScreen;
            Screen.fullScreen = fullScreen;
        }
    }

    #region playerList

    private void OnlistChange(SyncList<string>.Operation op, int itemIndex, string oldItem, string newItem)
    {
        RefreshPlayerList();
    }

    //builds and rebuilds the player list UI
    private void RefreshPlayerList()
    {
        if(playerCount != null)
        {
            //update player count
            playerCount.text = $"{players.Count}/{maxConns}";
        }

        //clear gameobject list
        foreach(GameObject listItem in playerList)
        {
            Destroy(listItem);
        }
        playerList.Clear();

        //rebuild list
        foreach(string playerName in players)
        {
            //create item in lobby list
            GameObject listItem = Instantiate(playerListItemPrefab);
            listItem.transform.SetParent(playerListParent);
            playerList.Add(listItem);

            //set item text
            listItem.transform.GetChild(0).GetComponent<Text>().text = playerName;
        }
    }

    #endregion

    private void ExitRoom()
    {
        //remove local player from player list on server
        LobbyPlayer.localPlayer.CmdRemovePlayer(Client.clientID, ClientManager.singleton.username);

        //only host can close room
        if (isServer)
        {
            ClientSend.CloseRoom(roomID);
        }
    }

    private void OnApplicationQuit()
    {
        ExitRoom();
    }


    [Server] public void StartGame()
    {
        ClientSend.CloseRoom(roomID);
        Game.singleton.StartGame();
        manager.ServerChangeScene(gameScene);
    }

    public void Back()
    {
        ExitRoom();
        manager.StopHost();
    }

    public void ToggleSettings()
    {
        lobbyActive = !lobbyActive;
        lobbyMenu.SetActive(lobbyActive);
        settingsMenu.SetActive(!lobbyActive);
    }
}
