using UnityEngine;

// puts a hurt box on the enemy weapon
public class DamageBox : MonoBehaviour
{
    public bool damaged;
    public float damage;
    // set up the damage numbers based on who owns this box (regular enemy or boss)
    private void Awake()
    {
        AiStats astats = GetComponentInParent<AiStats>();
        if (astats != null)
        {
            damage = astats.att;
        }
        else
        {
            damage = GetComponentInParent<BossStats>().att;
        }
    }

    // when the game object turns on (like when the attack starts)
    private void OnEnable()
    {
        // reset the flag so we can hit again
        Debug.Log($"[DamageBox] Object Enabled. Resetting 'damaged' to FALSE. Time: {Time.time}");
        damaged = false;
    }

    // when something touches the collider
    void OnTriggerEnter(Collider other)
    {
        PlayerCombat player = other.GetComponent<PlayerCombat>();

        // check if we hit the player
        if (player != null)
        {
            // only hit them if we haven't already done it this swing
            if (damaged == false)
            {
                // success, deal damage
                Debug.Log($"[DamageBox] HIT PLAYER! Dealing damage. Time: {Time.time}");
                damaged = true;
                player.takeDamage(damage);
            }
            else
            {
                // we already hit them once this animation, so ignore this
                Debug.Log($"[DamageBox] HIT BLOCKED. Already damaged this swing. Time: {Time.time}");
            }
        }
    }
}