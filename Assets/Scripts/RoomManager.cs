using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class RoomManager : NetworkRoomManager
{
    public static string roomID;

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        //free mouse cursor
        Cursor.lockState = CursorLockMode.None;

        //move to title scene
        SceneManager.LoadScene(offlineScene);
    }
}
