using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class ClientManager : MonoBehaviour
{
    [Scene] [SerializeField] private string loggedInScene;
    [Scene] [SerializeField] private string loggedOutScene;

    public static ClientManager singleton;

    public string username;
    public int colour1 = 0, colour2 = 4;

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
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
        if(singleton == this)
        {
            Client.Disconnect();
        }
    }

    public void LogIn(string username)
    {
        this.username = username;

        Debug.Log("Signed in as " + username);
        SceneManager.LoadScene(loggedInScene);
    }

    public void LogOut()
    {
        username = "";

        Debug.Log("Signed out");
        SceneManager.LoadScene(loggedOutScene);
    }
}
