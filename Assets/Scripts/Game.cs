//the main game manager script
//methods for OnStartGame and OnCatch are selected according to gamemode
//keeps track of connected players


using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;

public enum Mode
{
    Classic,
    Zombies,
    Elimination,
    Reverse,
    Random
}

public class Game : MonoBehaviour
{
    public Dictionary<NetworkConnection, string> players = new Dictionary<NetworkConnection, string>();
    public Dictionary<NetworkConnection, string> playersAlive = new Dictionary<NetworkConnection, string>();

    public static Game singleton;

    public delegate void StartCallback();
    public delegate void CatchCallback(Player catcher, Player caught);

    public CatchCallback OnPlayerCatch;
    public StartCallback OnStartGame;

    public Mode gameMode;
    private System.Random rng;
    public NetworkConnection chosenPlayer;
    public string disconnectText;
    private RoomManager settings;

    void EmptyFunc() { }

    void Awake()
    {
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

    [Server] public void SetGameMode()
    {
        gameMode = settings.gameMode;

        //pick mode if random
        if(gameMode == Mode.Random)
        {
            System.Array values = typeof(Mode).GetEnumValues();
            gameMode = (Mode)values.GetValue(rng.Next(values.Length - 1));
        }

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
    public void AddPlayer(NetworkConnection conn, string username)
    {
        players.Add(conn, username);
        playersAlive.Add(conn, username);

        //start game
        if(players.Count == settings.numPlayers)
        {
            StartGame();
        }
    }

    void StartGame()
    {
        string role = (gameMode == Mode.Reverse) ? "runner" : "catcher";

        //pick catcher from players
        chosenPlayer = players.Keys.ElementAt(rng.Next(players.Count));

        Player.localPlayer.RpcStartGame($"{players[chosenPlayer]} is the {role}", gameMode, settings.catchDistance, settings.staminaRate * 4);
    }

    //called when a player leaves the game
    public void OnPlayerDisconnect(NetworkConnection conn)
    {
        if(conn == Player.localPlayer.connectionToClient)
        {
            return;
        }

        Player.localPlayer.RpcBroadcastMessage($"{players[conn]} left the game!");
        players.Remove(conn);
        playersAlive.Remove(conn);

        //catcher quit
        if(conn == chosenPlayer)
        {
            Player.localPlayer.RpcOnGameEnd("GAME OVER");
        }

        //one player left on zombies
        else if(gameMode == Mode.Zombies && playersAlive.Count == 1)
        {
            Player.localPlayer.RpcOnGameEnd($"{playersAlive.Values.ElementAt(0)} won the match!");
            return;
        }

        //no players left on elimination
        else if(gameMode == Mode.Elimination && playersAlive.Count == 0)
        {
            Player.localPlayer.RpcOnGameEndElimination(true);
        }
    }

    #region start

    void ClassicStart()
    {
        StartCoroutine(Player.localPlayer.Timer(settings.gameTime));
    }

    void ZombiesStart()
    {
        playersAlive.Remove(chosenPlayer);
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
        chosenPlayer = caught.connectionToClient;
        caught.RpcOnCaught(catcher);
    }

    void ZombiesCatch(Player catcher, Player caught)
    {
        caught.isCatcher = true;
        playersAlive.Remove(caught.connectionToClient);

        chosenPlayer = null;
        if (playersAlive.Count == 1)
        {
            Player.localPlayer.RpcOnGameEnd($"{playersAlive.Values.ElementAt(0)} won the match!");
            return;
        }

        caught.RpcOnCaught(catcher);
    }

    void EliminationCatch(Player catcher, Player caught)
    {
        playersAlive.Remove(caught.connectionToClient);
        caught.RpcOnCaught(catcher);

        if (playersAlive.Count == 0)
        {
            Player.localPlayer.RpcOnGameEndElimination(true);
        }
    }

    void ReverseCatch(Player catcher, Player caught)
    {
        catcher.isCatcher = false;
        caught.isCatcher = true;
        chosenPlayer = catcher.connectionToClient;
        caught.RpcOnCaught(catcher);
        Player.localPlayer.RpcBroadcastMessage($"{playersAlive[chosenPlayer]} is now runner");
    }

    #endregion
}