using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// the big door that requires 3 keys to open
public class FortCube : MonoBehaviour, IInteractable
{
    [SerializeField] private PlayerStats stats;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject BossUI;
    [SerializeField] private TextMeshProUGUI puzzleText;
    [SerializeField] private TextMeshProUGUI combatText;
    [SerializeField] private TextMeshProUGUI platformText;
    [SerializeField] private AudioManager audioManager;
    public GameObject doorCanvas; // the menu showing which keys we have
    public PlayerUpgrades upgrades;

    public string GetDescription()
    {
        return "Enter Fortress";
    }

    // check keys and show the menu
    public void Interact()
    {
        audioManager.PlaySFX(audioManager.click);
        Time.timeScale = 0f;
        doorCanvas.SetActive(true);

        // show mouse
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // update colors: green if we have the key, red if missing
        if (upgrades.puzzleKey) puzzleText.color = Color.green;
        else puzzleText.color = Color.red;
        if (upgrades.combatKey) combatText.color = Color.green;
        else combatText.color = Color.red;
        if (upgrades.platformKey) platformText.color = Color.green;
        else platformText.color = Color.red;
    }

    // close the menu
    public void closePuzzle()
    {
        audioManager.PlaySFX(audioManager.close);
        Time.timeScale = 1f;
        doorCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // try to enter the next level
    public void openDoor()
    {
        audioManager.PlaySFX(audioManager.click);
        // only open if we have all keys
        if (upgrades.allKey)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}