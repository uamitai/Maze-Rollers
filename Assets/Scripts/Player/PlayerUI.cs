using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;
using Mirror;

//pause menu in player prefab
public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [Scene] [SerializeField] private string titleScene;

    public static bool pauseOn;
    private NetworkManager manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = NetworkManager.singleton;
        pauseOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            //on escape input
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        //toggle activeness of pause menu and update var
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        pauseOn = pauseMenu.activeSelf;
    }

    public void LeaveRoom()
    {
        //disconnect from match
        manager.StopHost();
    }
}
