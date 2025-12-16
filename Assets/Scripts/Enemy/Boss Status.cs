using TMPro;
using UnityEngine;
using UnityEngine.UI;

// handles the visual health bar for the boss
public class bossStatus : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;

    // update the bar and the text numbers
    public void updateHealthBar(float current, float max)
    {
        // change the slider value based on health percentage
        healthSlider.value = current / max;

        // show the health numbers but cut off the decimals so it looks clean
        healthText.text = current.ToString("F0") + "/" + max.ToString("F0");
    }
}