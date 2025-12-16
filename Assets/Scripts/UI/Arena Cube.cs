using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// the object that checks if you beat enough enemies
public class ArenaCube : MonoBehaviour, IInteractable
{
    [Header("UI Elements")]
    [SerializeField] private PlayerUpgrades upgrades;
    [SerializeField] private TextMeshProUGUI arenatext;
    public AudioManager audioManager;
    public GameObject arenaCanvas;

    public string GetDescription()
    {
        return "Obtain Combat Key";
    }

    // open the info screen
    public void Interact()
    {
        audioManager.PlaySFX(audioManager.click);
        Time.timeScale = 0f;
        arenaCanvas.SetActive(true);

        // let the mouse move
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        check();
    }

    // close the screen
    public void closePuzzle()
    {
        audioManager.PlaySFX(audioManager.close);
        Time.timeScale = 1f;
        arenaCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // update the text to say if you won or not
    public void check()
    {
        if (upgrades.combatKey)
        {
            arenatext.text = "Combat key unlocked!";
        }
        else
        {
            arenatext.text = "Kill All Enemies to Receieve \r\nCombat Key.";
        }
    }
}