using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// handles pausing the game and changing settings
public class PauseMenu : MonoBehaviour
{
    public static bool paused = false;
    public GameObject PauseMenuCanvas;

    [Header("UI Components")]
    public Slider music;
    public Slider sfx;

    [Header("References")]
    public AudioManager audioManager;

    // setup the sliders with current volume
    void Start()
    {
        Time.timeScale = 1f; // make sure game is running

        music.value = audioManager.getAudiolevel("music");
        sfx.value = audioManager.getAudiolevel("sfx");
        music.onValueChanged.AddListener(SetMusicVolume);
        sfx.onValueChanged.AddListener(SetSFXVolume);
    }

    // check for the escape key
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (paused)
            {
                Play();
            }
            else
            {
                Stop();
            }
        }
    }

    // adjust music volume
    public void SetMusicVolume(float value)
    {
        audioManager.setAudiolevel("music", value);
    }

    // adjust sfx volume
    public void SetSFXVolume(float value)
    {
        audioManager.setAudiolevel("sfx", value);
    }

    // pause the game
    public void Stop()
    {
        PauseMenuCanvas.SetActive(true);
        Time.timeScale = 0f; // freeze time
        paused = true;
        Cursor.lockState = CursorLockMode.None; // show cursor
        Cursor.visible = true;
    }

    // unpause the game
    public void Play()
    {
        PauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f; // unfreeze time
        paused = false;
        Cursor.lockState = CursorLockMode.Locked; // hide cursor
        Cursor.visible = false;
    }

    // go back to start screen
    public void MainMenuButton()
    {
        SceneManager.LoadScene(0);
    }

    // close the game
    public void Quit()
    {
        Application.Quit();
        Debug.Log("The Player has Quit the game");
    }
}