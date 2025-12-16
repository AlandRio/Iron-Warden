using UnityEngine;

// puts this on the player's weapon to hurt enemies
public class DealDamage : MonoBehaviour
{
    PlayerStats stats;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
    }

    // when the weapon hits something
    void OnTriggerEnter(Collider other)
    {
        // 1. check if we hit a normal enemy
        AiDamage enemy = other.GetComponent<AiDamage>();

        if (enemy != null)
        {
            enemy.takeDamage(stats.attack);
        }
        // 2. check if we hit the boss (needs a special tag)
        else if (other.CompareTag("BossDamage"))
        {
            // look for the boss script in the parent object
            BossDamage boss = other.GetComponentInParent<BossDamage>();

            if (boss != null)
            {
                boss.takeDamage(stats.attack);
            }
        }
    }
}