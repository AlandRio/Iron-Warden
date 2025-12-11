using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // ADD THIS to use TextMeshPro components

public class ColorChanger : MonoBehaviour, IInteractable
{
    [Header("UI Elements")]
    public GameObject puzzleCanvasObject; // The whole canvas to turn on/off
    public TMP_InputField playerInput;    // Drag your InputField here
    public TMP_Text resultText;           // Drag your "Top Text" here
    public PlayerUpgrades upgrades;

    public string GetDescription()
    {
        return "Open puzzle";
    }

    public void Interact()
    {
        Time.timeScale = 0f;
        puzzleCanvasObject.SetActive(true);

        // Optional: Unlock cursor so you can click the box
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void closePuzzle()
    {
        Time.timeScale = 1f;
        puzzleCanvasObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void submitPuzzle()
    {
        if (playerInput.text.ToLower() == "yellow")
        {
            resultText.text = "Correct! Unlocked Dash.";
            upgrades.unlockDash();
        }
        else
        {
            resultText.text = "Wrong!";
        }
    }
}