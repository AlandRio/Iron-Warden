using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] float damage = 50f;
    void OnTriggerEnter(Collider other)
    {
            PlayerCombat player = other.GetComponent<PlayerCombat>();

            // ONLY deal damage if the enemy script was actually found
            if (player != null)
            {
                player.takeDamage(damage);
            }
    }
}
