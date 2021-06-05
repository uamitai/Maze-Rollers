//script for room settings screen
//every button has a method called when pressed
//start function gets values, OKButton sets values


using UnityEngine;
using UnityEngine.UI;

public class RoomSettings : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider runSpeedSlider;
    [SerializeField] private Slider catchDistanceSlider;
    [SerializeField] private Slider staminaRateSlider;

    [Header("Text Objects")]
    [SerializeField] private Text gameModeText;
    [SerializeField] private Text gameTimeText;
    [SerializeField] private Text catchDistanceText;
    [SerializeField] private Text staminaRateText;

    private RoomManager manager;
    private Mode gameMode;
    private int gameTime;

    const int MAX_GAME_TIME = 8;
    const int SECONDS_IN_MINUTE = 60;

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
        gameTimeText.text = $"{manager.gameTime / SECONDS_IN_MINUTE}:00";
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
            gameMode = Mode.Random;
        }
        else
        {
            gameMode--;
        }

        gameModeText.text = gameMode.ToString();
    }

    public void GameModeRight()
    {
        if (gameMode == Mode.Random)
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
        gameTime -= SECONDS_IN_MINUTE;
        if(gameTime < SECONDS_IN_MINUTE)
        {
            gameTime = MAX_GAME_TIME * SECONDS_IN_MINUTE;
        }
        gameTimeText.text = $"{gameTime / SECONDS_IN_MINUTE}:00";
    }

    public void GameTimeRight()
    {
        gameTime += SECONDS_IN_MINUTE;
        if (gameTime > MAX_GAME_TIME * SECONDS_IN_MINUTE)
        {
            gameTime = SECONDS_IN_MINUTE;
        }
        gameTimeText.text = $"{gameTime / SECONDS_IN_MINUTE}:00";
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
