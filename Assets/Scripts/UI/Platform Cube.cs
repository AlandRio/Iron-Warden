using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// simple object that gives you the platform key when you touch it
public class PlatformCube : MonoBehaviour, IInteractable
{
    [Header("UI Elements")]
    [SerializeField] private PlayerUpgrades upgrades;
    public AudioManager audioManager;
    public GameObject platformCanvas; // the popup window

    // text that shows when you look at it
    public string GetDescription()
    {
        return "Obtain Platform Key";
    }

    // open the menu and give the key immediately
    public void Interact()
    {
        audioManager.PlaySFX(audioManager.click);
        Time.timeScale = 0f; // pause game
        platformCanvas.SetActive(true);

        // let the mouse move
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // actually unlock the ability
        upgrades.unlock("platform");
    }

    // close the popup
    public void closePuzzle()
    {
        audioManager.PlaySFX(audioManager.close);
        Time.timeScale = 1f; // unpause
        platformCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}