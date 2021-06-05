//the catching mechanic script
//implemented by sending raycast objects in the direction in front of the player
//if the raycast hits a remote player and the mouse is clicked, sends a command to the server to make a catch


using UnityEngine;
using Mirror;

[RequireComponent(typeof(Player))]
public class PlayerCatch : NetworkBehaviour
{
    [SerializeField] public float range;
    [SerializeField] private LayerMask mask;
    [SerializeField] private Sprite crosshairBlack;
    [SerializeField] private Sprite crosshairRed;

    private Transform cam;
    private RaycastHit hit;

    private const string playerLayer = "RemotePlayer";

    // Start is called before the first frame update
    void Start()
    {
        cam = Player.localPlayer.cam;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            //only local player could catch
            return;
        }

        if (Player.UI.pauseOn || !GetComponent<Player>().isAlive)
        {
            //player cannot catch on pause
            return;
        }

        Player.UI.crosshair.sprite = crosshairBlack;

        //check for raycast hit
        if (Physics.Raycast(cam.position, cam.forward, out hit, range, mask))
        {
            Catch();
        }
    }

    void Catch()
    {
        //if raycast hit a remote player
        if(hit.collider.gameObject.layer == LayerMask.NameToLayer(playerLayer))
        {
            //set crosshair
            Player.UI.crosshair.sprite = crosshairRed;

            //on local player input
            if (Input.GetButtonDown("Fire1"))
            {
                //send remote player's name
                CmdCatch(hit.collider.gameObject.GetComponent<Player>());
            }
        }
    }

    //sends command to server to catch player
    [Command] void CmdCatch(Player player)
    {
        if(!player.isCatcher)
        {
            Game.singleton.OnPlayerCatch(GetComponent<Player>(), player);
        }
    }
}
