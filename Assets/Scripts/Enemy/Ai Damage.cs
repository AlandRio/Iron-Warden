using UnityEngine;

public class AiDamage : MonoBehaviour
{
    private AiStats stats;
    private float currentHealth;
    [SerializeField] private EnemyHealth healthBar;
    // 1. Add this variable so you can drag your particle prefab here in the Inspector
    [SerializeField] private GameObject deathEffect;

    void Start()
    {
        stats = GetComponent<AiStats>();
        currentHealth = stats.hp;
        healthBar.updateHealthBar(currentHealth, stats.hp);
        healthBar = GetComponentInChildren<EnemyHealth>();
    }

    public void takeDamage(float damage)
    {


        currentHealth -= damage;
        healthBar.updateHealthBar(currentHealth, stats.hp);
        Debug.Log("I took " + damage + " damage and now my hp is " + currentHealth);
        if (currentHealth <= 0)
        {
            die();
        }
    }

    private void die()
    {
        Debug.Log("I died");
        if (deathEffect != null)
        {
            GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }
        Destroy(gameObject);
    }
}