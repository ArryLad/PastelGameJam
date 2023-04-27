using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Victory/GameOver")]
    public GameObject victoryScreen;
    public bool hasWon = false;
    public bool hasLost = false;
    public GameObject gameOverScreen;

    [Header("Pause")]
    public GameObject PauseScreen;
    public bool GameIsPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && (hasWon == false && hasLost == false)) //if the game has neither been won or lost, let the player pause with ESC
        {
            if (GameIsPaused) //if paused resume
            {
                Resume();
            }
            else //otherwise paused
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        //Deactivate pause screen, play music, set Time scale back to normal and now Game is not paused
        PauseScreen.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
    void Pause()
    {
        //Same as resume but opposite
        PauseScreen.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
}
