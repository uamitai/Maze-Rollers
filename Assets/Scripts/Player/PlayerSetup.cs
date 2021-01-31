using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

//seting up the player in the game
[RequireComponent(typeof(Player))]
public class PlayerSetup : NetworkBehaviour
{
    [SyncVar(hook = nameof(SetName))] private string username;

    [SerializeField] private Behaviour[] toEnable;
    [SerializeField] private Transform nameplate;
    [SerializeField] private GameObject Model;

    public static Transform cam;

    private const string playerLayer = "RemotePlayer";
    private const string modelLayer = "DontDraw";
    private const int camIndex = 1;

    // Start is called before the first frame update
    void Start()
    {
        Setup();

        if(isLocalPlayer)
        {
            //dont render model and nameplate
            Model.layer = LayerMask.NameToLayer(modelLayer);
            nameplate.gameObject.layer = LayerMask.NameToLayer(modelLayer);

            CmdSetName(ClientManager.inst.playerUsername);

            //set local player camera
            cam = transform.GetChild(camIndex);
        }
        else
        {
            //move to remote player layer
            gameObject.layer = LayerMask.NameToLayer(playerLayer);
        }
    }

    void LateUpdate()
    {
        //rotate name plate
        nameplate.LookAt(nameplate.position + cam.rotation * Vector3.forward,
            cam.rotation * Vector3.up);
    }

    [Command] private void CmdSetName(string _username)
    {
        username = _username;
    }

    private void SetName(string _, string _username)
    {
        transform.name = _username;
        nameplate.GetChild(0).GetComponent<Text>().text = _username;
    }

    public void Setup()
    {
        if(isLocalPlayer)
        {
            SetComponentsEnabled(true);
            GameManager.inst.SetCameraActive(false);
        }
    }

    public void SetComponentsEnabled(bool active)
    {
        //enable all scripts
        foreach (Behaviour component in toEnable)
        {
            component.enabled = active;
        }
    }

    void OnDisable()
    {
        if(isLocalPlayer)
        {
            //enable scene camera
            GameManager.inst.SetCameraActive(true);
        }
    }
}
