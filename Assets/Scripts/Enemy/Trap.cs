using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

// simple script for things like spikes or lava
public class Trap : MonoBehaviour
{
    [SerializeField] float damage = 50f;
    // when something touches the trap
    void OnTriggerEnter(Collider other)
    {
        PlayerCombat player = other.GetComponent<PlayerCombat>();

        // make sure it is actually the player stepping on it
        if (player != null)
        {
            player.takeDamage(damage);
        }
    }
}