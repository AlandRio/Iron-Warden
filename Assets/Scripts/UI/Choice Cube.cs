using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// the final choice object at the end of the game
public class ChoiceCube : MonoBehaviour, IInteractable
{
    [Header("UI Elements")]
    public GameObject puzzleCanvasObject; // the choice menu

    public AudioManager audioManager;
    public string GetDescription()
    {
        return "Decide his Fate";
    }

    // show the choice buttons
    public void Interact()
    {
        audioManager.PlaySFX(audioManager.click);
        Time.timeScale = 0f;
        puzzleCanvasObject.SetActive(true);

        // show the mouse
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // triggers the good ending
    public void killHim()
    {
        Ending.Instance.setGoodEnding(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }
    // triggers the bad ending
    public void spareHim()
    {
        Ending.Instance.setGoodEnding(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}