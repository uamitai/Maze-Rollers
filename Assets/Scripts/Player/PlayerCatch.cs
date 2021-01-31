using UnityEngine;
using UnityEngine.UI;
using Mirror;

//the player catching mechanic
public class PlayerCatch : NetworkBehaviour
{
    [SerializeField] private Transform cam;
    [SerializeField] private float range;
    [SerializeField] private LayerMask mask;
    [SerializeField] private Sprite crosshairBlack;
    [SerializeField] private Sprite crosshairRed;
    [SerializeField] private Transform playerUI;

    private RaycastHit hit;
    private Image image;
    private const string playerLayer = "RemotePlayer";

    // Start is called before the first frame update
    void Start()
    {
        image = playerUI.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerUI.pauseOn)
        {
            //player cannot catch on pause
            return;
        }

        image.sprite = crosshairBlack;

        //check for raycast hit
        if (Physics.Raycast(cam.position, cam.forward, out hit, range, mask))
        {
            Catch();
        }
    }

    void Catch()
    {
        if(!isLocalPlayer)
        {
            //only local player could catch
            return;
        }

        //if raycast hit a remote player
        if (hit.collider.gameObject.layer == LayerMask.NameToLayer(playerLayer))
        {
            //on local player input
            if (Input.GetButtonDown("Fire1"))
            {
                //send remote player's name
                CmdBroadcastCatch(hit.collider.gameObject);
            }

            image.sprite = crosshairRed;
        }
    }

    [Command] void CmdBroadcastCatch(GameObject player)
    {
        //gets player by collider name
        //calls RpcOnCatch on all clients
        player.GetComponent<Player>().RpcOnCatch();
    }
}
