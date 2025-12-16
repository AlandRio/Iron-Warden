using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// shows the story text and images at the end of the game
public class EndingMenu : MonoBehaviour
{
    [Header("UI References")]
    // 0-4 are good ending images, 5-9 are bad ending images
    [SerializeField] private GameObject[] storyPanels;
    [SerializeField] private TextMeshProUGUI pageText;
    [SerializeField] private GameObject selfObject;
    [SerializeField] private GameObject creditsObject;

    [Header("Settings")]
    [SerializeField] private bool goodEnding = false;

    // which page we are reading (0, 1, 2, 3...)
    private int currentPanelIndex = 0;

    // helps us skip to the bad images if needed
    private int startIdx = 0;
    private string[] stories;

    private void Start()
    {
        // --- check what ending we got ---
        if (Ending.Instance != null)
        {
            // use the saved ending from the previous scene
            goodEnding = Ending.Instance.goodEnding;
            Debug.Log("Ending Singleton found. Using global ending state: " + goodEnding);
        }
        else
        {
            // fallback if testing directly
            Debug.LogWarning("Ending Singleton NOT found. Using local Inspector fallback: " + goodEnding);
        }

        // setup the text
        stories = new string[5];

        if (goodEnding)
        {
            startIdx = 0; // start at the first image
            stories[0] = "The computation of war is complete. The Iron Warden has fallen, dismantled by the very spirit it sought to crush.";
            stories[1] = "Reconstruction protocols engaged. Happy wasted no cycles, immediately sowing life back into their once corrutped home.";
            stories[2] = "Biosignatures detected. The fauna have returned, stepping into a home finally purged of the Warden's industrial greed.";
            stories[3] = "A final honor. Hope is laid to rest, his legacy encoded eternally into the roots of the home he died to protect.";
            stories[4] = "SYSTEM STATUS: HARMONY RESTORED. The fortress of fear has been repurposed into a home for all.";
        }
        else
        {
            startIdx = 5; // skip to the bad images
            stories[0] = "CRITICAL FAILURE. 'I knew you were weak...' The audio file corrupted instantly. Happy's compassion was deemed a fatal error in the Warden's logic.";
            stories[1] = "System shutdown complete. Happy and Hope now lie dormant, their chassis rusting together beneath the fading memory of their favorite tree.";
            stories[2] = "But the hunger for control is a recursive loop with no exit. Despite total domination, the Warden's core burned for a power that did not exist.";
            stories[3] = "Resource depletion: 100%. The vibrant energy of the forest has been formatted into a wasteland; a void where no signal can survive.";
            stories[4] = "TIME INDEX: +50 YEARS. Status: Abandoned. The Warden has moved on, leaving behind a 'home' so corrupted that even the conqueror could not endure it.";
        }

        // start at page 0
        currentPanelIndex = 0;

        UpdatePanelDisplay();
    }

    // go to the next page
    public void Next()
    {
        // if we aren't at the end yet
        if (currentPanelIndex < stories.Length - 1)
        {
            currentPanelIndex++;
            UpdatePanelDisplay();
        }
        else
        {
            // finished the story, show credits
            selfObject.SetActive(false);
            creditsObject.SetActive(true);
        }
    }

    // go back a page
    public void Back()
    {
        // only if we aren't at the start
        if (currentPanelIndex > 0)
        {
            currentPanelIndex--;
            UpdatePanelDisplay();
        }
    }

    // updates the text and shows the correct image
    private void UpdatePanelDisplay()
    {
        // calculate which image to show based on ending type and page number
        int targetGameObjectIndex = startIdx + currentPanelIndex;

        // 1. update text
        if (pageText != null)
        {
            pageText.text = stories[currentPanelIndex];
        }

        // 2. update images
        // turn on only the one we want, turn off everything else
        for (int i = 0; i < storyPanels.Length; i++)
        {
            if (i == targetGameObjectIndex)
            {
                storyPanels[i].SetActive(true);
            }
            else
            {
                storyPanels[i].SetActive(false);
            }
        }
    }
}