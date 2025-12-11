using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public string playerTag = "Player";
    private bool isActivated = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (!isActivated)
            {
               PlayerStats player = other.GetComponent<PlayerStats>();
               player.spawnPoint = transform.position;
               isActivated = true;
            }
        }
    }
}
