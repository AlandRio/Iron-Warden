using TMPro;
using UnityEngine;
using UnityEngine.UI;

// handles the health bar plus the sprint and dash text on screen
public class playerStatus : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI sprintText;
    [SerializeField] private TextMeshProUGUI dashText;

    // update the red bar
    public void updateHealthBar(float current, float max)
    {
        // update the slider using the precise float value for smoothness
        healthSlider.value = current / max;

        // fix: use "F0" to format the number with 0 decimal places.
        // this turns "45.12345" into just "45".
        healthText.text = current.ToString("F0") + "/" + max.ToString("F0");
    }

    // shows arrows or hides them
    public void changeSprintText(string text)
    {
        sprintText.text = text;
    }

    // updates the countdown for the dash ability
    public void updateDashText(float current, float max, bool canDash, bool finishedDashing)
    {
        if (!canDash)
        {
            dashText.text = "";
        }
        else
        {
            if (finishedDashing)
            {
                dashText.text = "Dash Available";
            }
            else
            {
                // show how many seconds left
                dashText.text = "Dash - " + (max - current).ToString("F1") + " Seconds";
            }
        }
    }
}