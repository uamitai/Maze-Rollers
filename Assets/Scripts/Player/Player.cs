using System.Collections;
using UnityEngine;
using Mirror;

//defines the player as a part/unit of the game
[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour
{
    [SyncVar] private bool isAlive = true;

    private Transform spawnPoint;
    private Transform namePlate;
    private Collider col;

    // Start is called before the first frame update
    void Start()
    {
        isAlive = true;
        col = GetComponent<Collider>();
    }

    [ClientRpc] public void RpcOnCatch()
    {
        if(isLocalPlayer && isAlive)
        {
            isAlive = false;
            //disable scripts and enable main camera for local player
            GetComponent<PlayerSetup>().SetComponentsEnabled(false);
            GameManager.inst.SetCameraActive(true);
        }

        //disable collider and children for everyone
        ToggleOnCatch(false);

        StartCoroutine(Respawn());
    }

    void ToggleOnCatch(bool active)
    {
        //toggle collider
        col.enabled = active;

        //toggle children
        foreach(Transform child in transform)
        {
            if(child != transform)
            {
                child.gameObject.SetActive(active);
            }
        }
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3f);

        isAlive = true;
        GetComponent<PlayerSetup>().Setup();
        ToggleOnCatch(true);

        //place player at spawn point
        spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
    }
}
