using UnityEngine;
using Mirror;

public class LobbyPlayer : NetworkRoomPlayer
{
    public static LobbyPlayer localPlayer;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        localPlayer = this;
        CmdAddPlayer(Client.clientID, ClientManager.singleton.username);
    }

    [Command] public void CmdAddPlayer(int clientID, string username)
    {
        Debug.Log("added " + username);
        Lobby.singleton.players.Add(username);
        Game.singleton.players.Add(clientID, username);
    }

    [Command] public void CmdRemovePlayer(int clientID, string username)
    {
        Debug.Log("removed " + username);
        Lobby.singleton.players.Remove(username);
        Game.singleton.players.Remove(clientID);
    }
}
