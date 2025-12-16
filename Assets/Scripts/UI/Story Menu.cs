using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// shows the intro story panels before the game starts
public class StoryMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject[] storyPanels;
    [SerializeField] private TextMeshProUGUI pageText;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject storyMenu;

    private int currentPanelIndex = 0;
    private string[] stories;

    // setup the story text
    private void Start()
    {
        stories = new string[storyPanels.Length];
        stories[0] = "The forest was once a sanctuary. For units 'Happy' and 'Hope,' it was more than just a landscape—it was home.";
        stories[1] = "But peace is fragile. The Iron Warden, driven by a directive of absolute control, sought to consume the forest's resources.";
        stories[2] = "Hope was killed. Happy was buried beneath the ruin. The Warden’s Fortress seemed absolute over the once peaceful forest.";
        stories[3] = "Yet, in its final moments, Hope uploaded one last directive to its fallen friend. An infinite protocol: death would no longer be the end.";
        stories[4] = "INITIALIZING REBOOT... WAKE UP, HAPPY. RECLAIM OUR HOME.";
        UpdatePanelDisplay();
    }

    // go to the next page
    public void Next()
    {
        // check if we are at the last panel
        if (currentPanelIndex < storyPanels.Length - 1)
        {
            currentPanelIndex++;
            UpdatePanelDisplay();
        }
        else
        {
            // if done reading, start the game
            Play();
        }
    }

    // go back a page
    public void Back()
    {
        // check if we are not at the start
        if (currentPanelIndex > 0)
        {
            currentPanelIndex--;
            UpdatePanelDisplay();
        }
        else
        {
            // go back to main menu if we are on page 1
            mainMenu.SetActive(true);
            storyMenu.SetActive(false);
        }
    }

    // load the first level
    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // helper to turn the right image on and update text
    private void UpdatePanelDisplay()
    {
        for (int i = 0; i < storyPanels.Length; i++)
        {
            if (i == currentPanelIndex)
            {
                storyPanels[i].SetActive(true);
            }
            else
            {
                storyPanels[i].SetActive(false);
            }
        }
        if (pageText != null)
        {
            pageText.text = stories[currentPanelIndex];
        }
    }
}