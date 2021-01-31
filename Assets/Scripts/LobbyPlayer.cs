using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LobbyPlayer : NetworkRoomPlayer
{
    public static LobbyPlayer localPlayer;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        localPlayer = this;
        CmdAddPlayer(ClientManager.inst.playerUsername);
    }

    [Command] public void CmdAddPlayer(string username)
    {
        Debug.Log("added player " + username);
        Lobby.inst.players.Add(username);
    }

    [Command] public void CmdRemovePlayer(string username)
    {
        Debug.Log("removed player " + username);
        Lobby.inst.players.Remove(username);
    }
}
