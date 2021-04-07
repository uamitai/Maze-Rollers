using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;

public class Disconnect : MonoBehaviour
{
    [Scene] [SerializeField] private string titleScene;
    [SerializeField] private Text disconnectText;

    void Start()
    {
        disconnectText.text = Game.singleton.disconnectText;
    }

    public void ExitButton()
    {
        NetworkManager.singleton.offlineScene = titleScene;
        SceneManager.LoadScene(titleScene);
    }
}
