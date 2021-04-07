using UnityEngine;
using UnityEngine.UI;

public class RoomSettings : MonoBehaviour
{
    [SerializeField] private Slider runSpeedSlider;
    [SerializeField] private Slider catchDistanceSlider;
    [SerializeField] private Slider staminaRateSlider;

    [SerializeField] private Text gameModeText;
    [SerializeField] private Text gameTimeText;
    [SerializeField] private Text catchDistanceText;
    [SerializeField] private Text staminaRateText;

    private RoomManager manager;
    private Mode gameMode;
    private int gameTime;

    void Start()
    {
        //get values
        manager = RoomManager.singleton;
        gameMode = manager.gameMode;
        gameTime = manager.gameTime;
        catchDistanceSlider.value = manager.catchDistance;
        staminaRateSlider.value = manager.staminaRate;

        //set screen
        gameModeText.text = manager.gameMode.ToString();
        gameTimeText.text = $"{manager.gameTime / 60}:00";
    }

    void Update()
    {
        //update numbers according to sliders
        catchDistanceText.text = catchDistanceSlider.value.ToString();
        staminaRateText.text = staminaRateSlider.value.ToString();
    }

    //game mode buttons
    public void GameModeLeft()
    {
        if(gameMode == Mode.Classic)
        {
            gameMode = Mode.Reverse;
        }
        else
        {
            gameMode--;
        }

        gameModeText.text = gameMode.ToString();
    }

    public void GameModeRight()
    {
        if (gameMode == Mode.Reverse)
        {
            gameMode = Mode.Classic;
        }
        else
        {
            gameMode++;
        }

        gameModeText.text = gameMode.ToString();
    }

    //game time buttons
    public void GameTimeLeft()
    {
        gameTime -= 60;
        if(gameTime < 60)
        {
            gameTime = 8 * 60;
        }
        gameTimeText.text = $"{gameTime / 60}:00";
    }

    public void GameTimeRight()
    {
        gameTime += 60;
        if (gameTime > 8 * 60)
        {
            gameTime = 60;
        }
        gameTimeText.text = $"{gameTime / 60}:00";
    }

    public void OKBUtton()
    {
        //set values
        manager.gameMode = gameMode;
        manager.gameTime = gameTime;
        manager.catchDistance = catchDistanceSlider.value;
        manager.staminaRate = staminaRateSlider.value;
    }
}
