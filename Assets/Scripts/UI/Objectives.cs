using TMPro;
using UnityEngine;

// handles the list of goals on the screen
public class Objectives : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI firstText;
    [SerializeField] private TextMeshProUGUI secondText;
    [SerializeField] private TextMeshProUGUI thirdText;
    [SerializeField] private PlayerUpgrades upgrades;

    // turns the text green when you finish a task
    public void completeObjective(int number)
    {
        switch (number)
        {
            case 1: firstText.color = Color.green; break;
            case 2: secondText.color = Color.green; break;
            case 3: thirdText.color = Color.green; break;
            default:
                return;
        }
    }

}