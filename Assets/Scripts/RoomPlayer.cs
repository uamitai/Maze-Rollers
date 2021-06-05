//script derived from NetworkRoom player to override it
//each player that joins a room gets an instance of this class
//local player uses command to add and remove self from lobby players list


using UnityEngine;
using Mirror;

public class RoomPlayer : NetworkRoomPlayer
{
    public static RoomPlayer localPlayer;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        localPlayer = this;
        CmdAddPlayer(ClientManager.singleton.username);
    }

    [Command] public void CmdAddPlayer(string username)
    {
        Debug.Log("added " + username);
        Room.singleton.players.Add(username);
    }

    [Command] public void CmdRemovePlayer(string username)
    {
        Debug.Log("removed " + username);
        Room.singleton.players.Remove(username);
    }
}
