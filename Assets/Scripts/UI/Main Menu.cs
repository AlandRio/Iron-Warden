using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// handles the start screen and options
public class MainMenu : MonoBehaviour
{
    public Slider music1;
    public Slider music2;
    public Slider sfx1;
    public Slider sfx2;
    public AudioManager audioManager;

    // setup the sliders to match current volume
    public void Start()
    {
        music1.value = audioManager.getAudiolevel("music");
        sfx1.value = audioManager.getAudiolevel("sfx");
        music1.onValueChanged.AddListener(SetMusicVolume);
        sfx1.onValueChanged.AddListener(SetSFXVolume);

        music2.value = audioManager.getAudiolevel("music");
        sfx2.value = audioManager.getAudiolevel("sfx");
        music2.onValueChanged.AddListener(SetMusicVolume);
        sfx2.onValueChanged.AddListener(SetSFXVolume);
    }

    // adjust music volume
    public void SetMusicVolume(float value)
    {
        audioManager.setAudiolevel("music", value);
    }

    // adjust sound effects volume
    public void SetSFXVolume(float value)
    {
        audioManager.setAudiolevel("sfx", value);
    }

    // close the game
    public void Quit()
    {
        audioManager.PlaySFX(audioManager.close);
        Application.Quit();
        Debug.Log("The Player has Quit the game");
    }
}