using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

//lobby scene script
public class Lobby : NetworkBehaviour
{
    [Scene] [SerializeField] private string gameScene;

    [SerializeField] private GameObject startButton;
    [SerializeField] private Text roomName;
    [SerializeField] private Text playerCount;
    [SerializeField] private Transform playerListParent;
    [SerializeField] private GameObject playerListItemPrefab;

    public SyncList<string> players = new SyncList<string>();
    [SyncVar] private string roomID;
    [SyncVar] private int maxConns;

    public static Lobby inst;

    private NetworkManager manager;
    private List<GameObject> playerList;

    // Start is called before the first frame update
    void Start()
    {
        inst = this;
        manager = NetworkManager.singleton;
        playerList = new List<GameObject>();

        if(isServer)
        {
            roomID = RoomManager.roomID;
            maxConns = manager.maxConnections + 1;
        }

        //only host can view start button
        startButton.SetActive(isServer);
        roomName.text = "ROOM CODE: " + roomID;

        players.Callback += OnlistChange;
    }

    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            Back();
        }
    }

    #region playerList

    private void OnlistChange(SyncList<string>.Operation op, int itemIndex, string oldItem, string newItem)
    {
        RefreshPlayerList();
    }

    private void RefreshPlayerList()
    {
        Debug.Log("refreshing player list...");
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
        LobbyPlayer.localPlayer.CmdRemovePlayer(ClientManager.inst.playerUsername);

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


    public void StartGame()
    {
        ExitRoom();
        manager.ServerChangeScene(gameScene);
    }

    public void Back()
    {
        ExitRoom();
        manager.StopHost();
    }
}
