using TMPro;
using UnityEngine;

// changes the objective text color when you reach the boss
public class BossCheckpoint : MonoBehaviour
{
    public TextMeshProUGUI objective;
    public string playerTag = "Player";
    private bool isActivated = false;

    // when the player walks into this zone
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            // only do this once
            if (!isActivated)
            {
                objective.color = Color.green; // make the text green to show it's done
                isActivated = true;
            }
        }
    }
}