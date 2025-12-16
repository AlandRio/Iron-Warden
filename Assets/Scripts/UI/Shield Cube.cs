using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// the puzzle that breaks the boss's shield
public class ShieldCube : MonoBehaviour, IInteractable
{
    [Header("UI Elements")]
    public GameObject puzzleCanvasObject; // the menu
    public TMP_InputField playerInput;    // where you type
    public TMP_Text resultText;           // tells you if you got it right
    public TMP_Text questionText;
    public string question;
    public string answer;
    public AudioManager audioManager;
    public BossTarget boss;
    public TMP_Text objectiveText; // updates the main goal text

    public string GetDescription()
    {
        return "Destroy Shield";
    }

    // open the puzzle
    public void Interact()
    {
        audioManager.PlaySFX(audioManager.click);
        Time.timeScale = 0f;
        puzzleCanvasObject.SetActive(true);
        questionText.text = question;

        // show mouse
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // close menu
    public void closePuzzle()
    {
        audioManager.PlaySFX(audioManager.close);
        Time.timeScale = 1f;
        puzzleCanvasObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // check if the answer is correct
    public void submitPuzzle()
    {
        audioManager.PlaySFX(audioManager.click);
        // ignore capital letters and spaces
        string cleanInput = playerInput.text.Trim().ToLower();
        string cleanAnswer = answer.Trim().ToLower();

        if (cleanInput == cleanAnswer)
        {
            // success message
            resultText.text = "Its me hope! This is the last thing i can do before i pass away forever\ngive em hell for me happy!";
            // update the player's instructions
            objectiveText.text = "Iron warden is\nNo longer Shielded!\nYou can now\nWalk into him\nTo Damage him";
            // actually break the shield on the boss
            boss.DestroyShield();
        }
        else
        {
            resultText.text = "Wrong!";
        }
    }
}