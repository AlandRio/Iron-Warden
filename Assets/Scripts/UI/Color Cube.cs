using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // needed for text things

// this is the object that gives you a puzzle to solve
public class ColorCube : MonoBehaviour, IInteractable
{
    [Header("UI Elements")]
    public GameObject puzzleCanvasObject; // the menu that pops up
    public TMP_InputField playerInput;    // where you type the answer
    public TMP_Text resultText;           // says if you were right or wrong
    public TMP_Text questionText;
    public string question;
    public string answer;
    public PlayerUpgrades upgrades;
    public string unlockable; // what ability this gives you
    public AudioManager audioManager;
    public GameObject BossUi;

    // text that shows up when looking at it
    public string GetDescription()
    {
        return "Open puzzle";
    }

    // open the puzzle menu
    public void Interact()
    {
        audioManager.PlaySFX(audioManager.click);
        Time.timeScale = 0f; // pause the game
        puzzleCanvasObject.SetActive(true);
        questionText.text = question;

        // let the mouse move freely
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // hide the menu and go back to game
    public void closePuzzle()
    {
        audioManager.PlaySFX(audioManager.close);
        Time.timeScale = 1f; // unpause
        puzzleCanvasObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // check if the typed answer is correct
    public void submitPuzzle()
    {
        audioManager.PlaySFX(audioManager.click);
        // clean up the text so capitalization doesn't matter
        string cleanInput = playerInput.text.Trim().ToLower();
        string cleanAnswer = answer.Trim().ToLower();

        if (cleanInput == cleanAnswer)
        {
            // special check if this is the boss door
            if (unlockable == "Door")
            {
                resultText.color = Color.green;
                BossUi.SetActive(true);
                closePuzzle();
                Destroy(gameObject);
            }
            else
            {
                // give the player the upgrade
                upgrades.unlock(unlockable.ToLower());

                // check if they found the puzzle key
                if (upgrades.allUnlocked)
                {
                    resultText.text = "All Movement Unlocked! u have been granted Puzzle key!";
                    upgrades.unlock("puzzle");
                }
                else
                {
                    resultText.text = "Correct! Unlocked " + unlockable;
                }
            }
        }
        else
        {
            resultText.text = "Wrong!";
        }
    }
}