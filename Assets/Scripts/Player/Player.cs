//main player script to execute commands and clientRpc's
//enables/disables the player children and components (only local player has playercontroller enabled, if catcher then also playercatch)
//syncs variables for the player across all connected clients, for example the username
//also rotates the nameplate towards the local player in the update method


using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Player : NetworkBehaviour
{
    //player variables to sync
    [SyncVar] public bool isAlive;
    [SyncVar(hook = nameof(SetName))] public string username;
    [SyncVar(hook = nameof(SetCatcher))] public bool isCatcher;
    [SyncVar(hook = nameof(DisplayTime))] private float timeRemaining;

    [Header("References to player GameObject")]
    [SerializeField] private GameObject UIPrefab;
    [SerializeField] private GameObject model;
    [SerializeField] public Transform cam;
    [SerializeField] private Transform nameplate;

    //static variables
    public static PlayerUI UI;
    public static Player localPlayer;
    public static Mode gameMode;
    private static Game GameManager;
    private SpawnPoint spawnPoint;

    private const string playerLayer = "RemotePlayer";
    private const string modelLayer = "DontDraw";

    // Start is called before the first frame update
    void Start()
    {
        isAlive = false;
        Setup(true);

        GameManager = Game.singleton;
        spawnPoint = new SpawnPoint(transform);

        if(isLocalPlayer)
        {
            localPlayer = this;

            //set up UI
            UI = Instantiate(UIPrefab).GetComponent<PlayerUI>();

            //dont render model and nameplate
            model.layer = LayerMask.NameToLayer(modelLayer);
            nameplate.gameObject.layer = LayerMask.NameToLayer(modelLayer);

            //set username
            CmdSetName(ClientManager.singleton.username);
        }
        else
        {
            //move to remote player layer
            gameObject.layer = LayerMask.NameToLayer(playerLayer);
            cam.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        //rotate name plate towards local player
        nameplate.LookAt(nameplate.position + localPlayer.cam.rotation * Vector3.forward, localPlayer.cam.rotation * Vector3.up);
    }

    //called when all players are ready
    [ClientRpc] public void RpcStartGame(string msg, Mode _gameMode, float catchDistance, float StaminaRate)
    {
        gameMode = _gameMode;
        localPlayer.GetComponent<PlayerController>().staminaSpeed = StaminaRate;
        localPlayer.GetComponent<PlayerCatch>().range = catchDistance;

        StartCoroutine(UI.CountDown(_gameMode, msg));

        localPlayer.CmdSendColours(ClientManager.singleton.colour1, ClientManager.singleton.colour2);
    }

    //send colours to server
    [Command] void CmdSendColours(int colour1, int colour2)
    {
        RpcRenderColours(MainMenu.colours[colour1], MainMenu.colours[colour2]);

        isCatcher = (connectionToClient == GameManager.chosenPlayer) ^ (GameManager.gameMode == Mode.Reverse);
    }

    //apply colours on all clients
    [ClientRpc] void RpcRenderColours(Color colour1, Color colour2)
    {
        model.GetComponent<PlayerColours>().RenderMaterial(colour1, colour2);
    }

    #region SetupComponents

    //sets up components and gameobjects of player
    void Setup(bool active)
    {
        model.SetActive(active);
        nameplate.gameObject.SetActive(active);

        if (!isLocalPlayer)
        {
            //move remote player to layer
            gameObject.layer = active ? LayerMask.NameToLayer(playerLayer) : LayerMask.NameToLayer(modelLayer);
        }
    }

    //enable controller and cather scripts
    public void EnableComponents(bool active)
    {
        isAlive = active;
        if (isLocalPlayer)
        {
            GetComponent<PlayerController>().enabled = active;
            GetComponent<PlayerCatch>().enabled = isCatcher && active;
            UI.crosshair.enabled = isCatcher && active;
        }
    }

    //sync name for all players
    [Command] void CmdSetName(string _username)
    {
        username = _username;
        GameManager.AddPlayer(connectionToClient, _username);
    }

    void SetColours(int colour1, int colour2)
    {
        model.GetComponent<PlayerColours>().RenderMaterial(MainMenu.colours[colour1], MainMenu.colours[colour2]);
    }

    void SetName(string _, string _username)
    {
        transform.name = _username;
        nameplate.GetChild(0).GetComponent<Text>().text = _username;
    }

    void SetCatcher(bool _, bool _isCatcher)
    {
        //enable catcher script
        if (isLocalPlayer)
        {
            GetComponent<PlayerCatch>().enabled = _isCatcher && isAlive;
            UI.crosshair.enabled = _isCatcher && isAlive;
        }

        //set nameplate color
        nameplate.GetChild(0).GetComponent<Text>().color = _isCatcher ? Color.red : Color.black;
    }

    #endregion

    [Server] public IEnumerator Timer(int time)
    {
        timeRemaining = time;
        while (timeRemaining > 0)
        {
            timeRemaining -= 1;
            yield return new WaitForSeconds(1f);
        }

        string msg = "";
        switch (gameMode)
        {
            case Mode.Classic:
                msg = $"{GameManager.players[GameManager.chosenPlayer]} lost the match!";
                break;

            case Mode.Reverse:
                msg = $"{GameManager.players[GameManager.chosenPlayer]} won the match!";
                break;

            case Mode.Elimination:
                RpcOnGameEndElimination(false);
                yield break;
        }
        RpcOnGameEnd(msg);
    }

    void DisplayTime(float _, float timeToDisplay)
    {
        timeToDisplay++;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        //last minute timer color turns red
        if (minutes == 0)
        {
            UI.timerText.color = Color.red;
        }

        UI.timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    [ClientRpc] public void RpcBroadcastMessage(string msg)
    {
        UI.AddKillToFeed(msg);
    }

    //called when player is caught
    [ClientRpc] public void RpcOnCaught(Player catcher)
    {
        UI.AddKillToFeed($"{catcher.username} caught {username}!");

        //setup caught screen and start respawning
        if (isLocalPlayer)
        {
            //unpause
            if(UI.pauseOn)
            {
                UI.TogglePauseMenu();
            }

            //free mouse cursor
            Cursor.lockState = CursorLockMode.None;

            StartCoroutine(UI.CaughtScreen(catcher.username));
        }

        Setup(false);
        EnableComponents(false);
    }

    //respawn a caught player
    [Command] public void CmdRespawn()
    {
        RpcRespawn();
    }

    [ClientRpc]
    void RpcRespawn()
    {
        if (gameMode != Mode.Elimination)
        {
            Setup(true);
        }
        else
        {
            UI.AddKillToFeed($"{username} is out!");
        }
        EnableComponents(true);

        //write to kill feed
        if (isCatcher && gameMode != Mode.Reverse)
        {
            UI.AddKillToFeed($"{username} is now catcher");
        }

        if (isLocalPlayer)
        {
            //lock mouse cursor
            Cursor.lockState = CursorLockMode.Locked;
        }

        transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
    }

    [ClientRpc] public void RpcOnGameEnd(string msg)
    {
        //disable UI
        Cursor.lockState = CursorLockMode.None;

        GameManager.disconnectText = msg;
        NetworkManager.singleton.StopHost();
    }

    [ClientRpc] public void RpcOnGameEndElimination(bool catcherWon)
    {
        //determine victory text if timer ran out on elimination
        string status = "";

        if (localPlayer.isCatcher)
        {
            status = catcherWon ? "won" : "lost";
        }
        else
        {
            status = localPlayer.model.activeSelf ? "won" : "lost";
        }

        //disable UI
        Cursor.lockState = CursorLockMode.None;

        GameManager.disconnectText = $"You {status} the match!";
        NetworkManager.singleton.StopHost();
    }
}

//respawn player at initial transform
class SpawnPoint
{
    public Vector3 position;
    public Quaternion rotation;

    public SpawnPoint(Transform t)
    {
        position = t.position;
        rotation = t.rotation;
    }
}