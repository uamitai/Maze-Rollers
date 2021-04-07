using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

//manages UI
public class PlayerUI : NetworkBehaviour
{
    [Header("Screens")]
    [SerializeField] private GameObject caughtScreen;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] public GameObject killFeed;

    [Header("Texts")]
    [SerializeField] private Text countdownText;
    [SerializeField] private Text caughtByText;
    [SerializeField] private Text respawnText;
    [SerializeField] public Text timerText;

    [Header("Other")]
    [SerializeField] private GameObject killFeedItemPrefab;
    [SerializeField] private Transform staminaMeter;
    [Scene] [SerializeField] private string titleScene;
    [Scene] [SerializeField] private string disconnectScene;

    public Image crosshair;
    public bool pauseOn;

    private NetworkManager manager;

    // Start is called before the first frame update
    void Start()
    {
        pauseOn = false;
        manager = NetworkManager.singleton;
        Cursor.lockState = CursorLockMode.Locked;
        manager.offlineScene = disconnectScene;
    }

    void Update()
    {
        //pause game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    #region coroutines

    //displays series of messages at start of game
    public IEnumerator CountDown(string msg)
    {
        if(!countdownText.enabled)
        {
            yield break;
        }

        countdownText.text = msg;
        yield return new WaitForSeconds(2f);

        int countdown = 3;
        while(countdown > 0)
        {
            countdownText.text = countdown.ToString();
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        Player.localPlayer.EnableComponents(true);
        countdownText.text = "GO!";
        yield return new WaitForSeconds(1.5f);

        countdownText.enabled = false;
        Game.singleton.OnStartGame();
    }

    public IEnumerator CaughtScreen(string catcher)
    {
        caughtScreen.SetActive(true);
        caughtByText.text = $"Caught by {catcher}!";

        //countdown until respawn
        for (int i = 0; i < 4; i++)
        {
            respawnText.text = $"Respawning in {4 - i}...";
            yield return new WaitForSeconds(1f);
        }

        caughtScreen.SetActive(false);
        Player.localPlayer.CmdRespawn();
    }

    IEnumerator Disconnect(float time)
    {
        //we're waiting to disconnect so we can tell the host
        yield return new WaitForSeconds(time);
        manager.StopHost();
    }

    #endregion

    //toggle activeness of pause menu
    public void TogglePauseMenu()
    {
        Player player = Player.localPlayer;

        //player must be alive
        if(!player.isAlive)
        {
            return;
        }

        pauseOn = !pauseOn;
        pauseMenu.SetActive(pauseOn);
        Cursor.lockState = pauseOn ? CursorLockMode.None : CursorLockMode.Locked;
    }

    //called from leave room button on pause menu
    public void LeaveRoom()
    {
        NetworkManager.singleton.offlineScene = titleScene;
        Player.localPlayer.CmdLeaveRoom();

        //disconnect from match
        StartCoroutine(Disconnect(0.25f));
    }

    //receives message and displays them on kill feed
    public void AddKillToFeed(string msg)
    {
        GameObject item = Instantiate(killFeedItemPrefab, killFeed.transform);
        item.GetComponentInChildren<Text>().text = msg;

        //destroy message
        Destroy(item, 6f);
    }

    public void SetStamina(float value)
    {
        staminaMeter.localScale = new Vector3(1f, value, 1f);
    }
}
