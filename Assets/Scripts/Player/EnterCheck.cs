using TMPro;
using UnityEngine;

// locks the player in the boss room and starts the fight timers
public class EnterCheckpoint : MonoBehaviour
{
    public GameObject Door; // the wall that appears behind you
    public string playerTag = "Player";
    private bool isActivated = false;
    public BossTarget boss; // reference to the boss script

    // when player enters the arena
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (!isActivated)
            {
                // close the door
                Door.SetActive(true);
                // tell the boss the fight has started
                boss.setPlatformTimerActive();
                isActivated = true;
            }
        }
    }
}