using UnityEngine;

// saves where the player respawns
public class Checkpoint : MonoBehaviour
{
    public string playerTag = "Player";
    private bool isActivated = false;

    // when player walks into this
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (!isActivated)
            {
                // tell the player script that this is the new home base
                PlayerStats player = other.GetComponent<PlayerStats>();
                player.spawnPoint = transform.position;
                isActivated = true;
            }
        }
    }
}