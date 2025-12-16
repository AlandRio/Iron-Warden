using UnityEngine;

// controls the enemy health and getting hit
public class AiDamage : MonoBehaviour
{
    private AiStats stats;
    private float currentHealth;
    [SerializeField] private EnemyHealth healthBar;
    // drag the particle effect here in the inspector
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private PlayerUpgrades upgrades;
    public AudioManager audioManager;
    public bool arena;

    // setting things up when the game starts
    void Start()
    {
        stats = GetComponent<AiStats>();
        currentHealth = stats.hp;
        healthBar.updateHealthBar(currentHealth, stats.hp);
        healthBar = GetComponentInChildren<EnemyHealth>();
    }

    // function for when the enemy gets hurt
    public void takeDamage(float damage)
    {


        // lower the health
        currentHealth -= damage;
        // update the visual bar
        healthBar.updateHealthBar(currentHealth, stats.hp);
        Debug.Log("I took " + damage + " damage and now my hp is " + currentHealth);
        // check if they ran out of health
        if (currentHealth <= 0)
        {
            die();
        }
    }

    // what happens when health hits zero
    private void die()
    {
        Debug.Log("I died");
        // if we are in the arena, tell the upgrades script
        if (arena)
        {
            upgrades.arenaDeath();
        }
        // make the cool explosion effect
        if (deathEffect != null)
        {
            GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
            // clean up the effect after 2 seconds so it doesn't stay forever
            Destroy(effect, 2f);
        }
        // play the death sound and remove this enemy
        audioManager.PlaySFX(audioManager.death);
        Destroy(gameObject);
    }
}