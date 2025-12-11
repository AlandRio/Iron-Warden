using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class playerHealth : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;

    void Start()
    {
        
    }

    public void updateHealthBar(float current, float max)
    {
        healthSlider.value = current / max;
        healthText.text = current.ToString() + "/" + max.ToString();
     }

    void LateUpdate()
    {

    }
}