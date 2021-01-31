using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Mirror;

public class ClientManager : MonoBehaviour
{
    [Scene] [SerializeField] private string loggedInScene;
    [Scene] [SerializeField] private string loggedOutScene;

    public static ClientManager inst;

    public string playerUsername { get; protected set; }
    private string playerPassword = "";
    public bool isLoggedIn { get; protected set; }

    private const float timeToLive = 5;

    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Client.Start();
    }

    private void Update()
    {
        ThreadManager.UpdateMain();
    }

    private void OnDestroy()
    {
        if(inst == this)
        {
            Client.Disconnect();
        }
    }

    public void LogIn(string username, string password)
    {
        if (!isLoggedIn)
        {
            playerUsername = username;
            playerPassword = password;
            isLoggedIn = true;

            Debug.Log("Signed in as " + username);
            SceneManager.LoadScene(loggedInScene);
        }
    }

    public void LogOut()
    {
        if (isLoggedIn)
        {
            playerUsername = "";
            playerPassword = "";
            isLoggedIn = false;

            Debug.Log("Signed out");
            SceneManager.LoadScene(loggedOutScene);
        }
    }
}
