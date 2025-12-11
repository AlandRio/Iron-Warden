using UnityEngine;

public class DealDamage : MonoBehaviour
{
    PlayerStats stats;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
    }

    void OnTriggerEnter(Collider other)
    {
        // Try to get the component
        AiDamage enemy = other.GetComponent<AiDamage>();

        // ONLY deal damage if the enemy script was actually found
        if (enemy != null)
        {
            enemy.takeDamage(stats.attack);
        }
    }
    void Update()
    {
        
    }
}
