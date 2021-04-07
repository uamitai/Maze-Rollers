using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;

public enum Mode
{
    Classic,
    Zombies,
    Elimination,
    Reverse
}

public class Game : MonoBehaviour
{
    public Dictionary<int, string> players = new Dictionary<int, string>();

    public static Game singleton;

    public delegate void StartCallback();
    public delegate void CatchCallback(Player catcher, Player caught);

    public CatchCallback OnPlayerCatch;
    public StartCallback OnStartGame;

    public Mode gameMode;
    private int playersReady;
    private System.Random rng;
    public int chosenPlayer;
    public string disconnectText;
    private RoomManager settings;

    void EmptyFunc() { }

    void Awake()
    {
        playersReady = 0;
        rng = new System.Random();
        OnStartGame = EmptyFunc;
        settings = RoomManager.singleton;
        disconnectText = "Disconnected from host!";

        if (singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Server] public void StartGame()
    {
        //pick catcher from players
        chosenPlayer = players.Keys.ElementAt(rng.Next(players.Count));
        gameMode = settings.gameMode;

        //set gamemode
        switch (gameMode)
        {
            case Mode.Classic:
                OnStartGame = ClassicStart;
                OnPlayerCatch = ClassicCatch;
                break;

            case Mode.Zombies:
                OnStartGame = ZombiesStart;
                OnPlayerCatch = ZombiesCatch;
                break;

            case Mode.Elimination:
                OnStartGame = EliminationStart;
                OnPlayerCatch = EliminationCatch;
                break;

            case Mode.Reverse:
                OnStartGame = ClassicStart;
                OnPlayerCatch = ReverseCatch;
                break;
        }
    }

    //called when player is ready
    //starts game when everyone is ready
    public void AddPlayer()
    {
        playersReady++;

        //start game
        if(playersReady == players.Count)
        {
            string role = (gameMode == Mode.Reverse) ? "runner" : "catcher";
            Player.localPlayer.RpcStartGame($"{players[chosenPlayer]} is the {role}!", gameMode, settings.catchDistance, settings.staminaRate);
        }
    }

    //called when a player leaves the game
    public void OnPlayerDisconnect(int clientID)
    {
        Player.localPlayer.RpcBroadcastMessage($"{players[clientID]} left the game!");

        if(clientID == chosenPlayer)
        {
            Player.localPlayer.RpcOnGameEnd("GAME OVER");
        }
        players.Remove(clientID);
    }

    #region start

    void ClassicStart()
    {
        StartCoroutine(Player.localPlayer.Timer(settings.gameTime));
    }

    void ZombiesStart()
    {
        players.Remove(chosenPlayer);
    }

    void EliminationStart()
    {
        ZombiesStart();
        ClassicStart();
    }

    #endregion
    #region catch

    void ClassicCatch(Player catcher, Player caught)
    {
        catcher.isCatcher = false;
        caught.isCatcher = true;
        chosenPlayer = caught.clientID;
        caught.RpcOnCaught(catcher);
    }

    void ZombiesCatch(Player catcher, Player caught)
    {
        caught.isCatcher = true;
        players.Remove(caught.clientID);
        if (players.Count == 1)
        {
            Player.localPlayer.RpcOnGameEnd($"{players.Values.ElementAt(0)} won the match!");
            return;
        }

        caught.RpcOnCaught(catcher);
    }

    void EliminationCatch(Player catcher, Player caught)
    {
        players.Remove(caught.clientID);
        caught.RpcOnCaught(catcher);

        if (players.Count == 0)
        {
            Player.localPlayer.RpcOnGameEnd("C");
        }
    }

    void ReverseCatch(Player catcher, Player caught)
    {
        catcher.isCatcher = false;
        caught.isCatcher = true;
        chosenPlayer = catcher.clientID;
        caught.RpcOnCaught(catcher);
        Player.localPlayer.RpcBroadcastMessage($"{players[chosenPlayer]} is now runner");
    }

    #endregion
}