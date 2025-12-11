using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class PlayerCombat : MonoBehaviour
{
    private Animator m_Animator;
    private PlayerStats stats;
    private PlayerInput playerInput; // Reference to the Input System
    private bool hasMeleed = false;
    private bool hasRanged = false;
    private float meleeTimer = 0f;
    private float rangedTimer = 0f;

    public Collider weaponCollider;
    private float currentHealth;
    [SerializeField] private playerHealth healthBar;

    void Start()
    {
        m_Animator = GetComponentInChildren<Animator>();
        stats = GetComponent<PlayerStats>();

        // Automatically find the PlayerInput component on this object
        playerInput = GetComponent<PlayerInput>();

        currentHealth = stats.health;
    }

    public void OnMelee(InputAction.CallbackContext context)
    {
        // If input is disabled (dead), ignore button presses
        if (playerInput != null && !playerInput.enabled) return;

        if (context.performed && !hasMeleed)
        {
            // Debug.Log("Melee Button Pressed!"); 
            m_Animator.SetTrigger("Melee");
            weaponCollider.enabled = true;
            hasMeleed = true;
        }
    }

    public void OnRanged(InputAction.CallbackContext context)
    {
        // If input is disabled (dead), ignore button presses
        if (playerInput != null && !playerInput.enabled) return;

        if (context.performed && !hasRanged)
        {
            m_Animator.SetTrigger("Ranged");
            hasRanged = true;
        }
    }

    public void takeDamage(float damage)
    {
        if (currentHealth - damage > 0)
        {
            m_Animator.SetTrigger("Hit");
        }
        currentHealth -= damage;

        // Ensure health bar updates if assigned
        if (healthBar != null)
            healthBar.updateHealthBar(currentHealth, stats.health);

        // Debug.Log("I took " + damage + " damage. HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            die();
        }
    }

    public void die()
    {
        // 1. Disable controls immediately
        if (playerInput != null) playerInput.DeactivateInput();

        // 2. Play death animation
        m_Animator.SetTrigger("Die");

        // 3. Wait 2 seconds, then call FinishDying
        Invoke("FinishDying", 2f);
    }

    // Runs automatically after 2 seconds
    private void FinishDying()
    {
        // Calls the method in PlayerStats that shows the cursor/game over menu
        stats.die();
    }

    // Call this from your gameOver script
    public void RespawnInput()
    {
        currentHealth = stats.health;

        if (healthBar != null)
            healthBar.updateHealthBar(currentHealth, stats.health);

        // Reset Animation
        m_Animator.Play("Idle");
        m_Animator.ResetTrigger("Die");

        // Re-enable controls
        if (playerInput != null) playerInput.ActivateInput();
    }

    void Update()
    {
        if (hasMeleed)
        {
            if (meleeTimer > 0.5f)
            {
                weaponCollider.enabled = false;
            }
            if (meleeTimer > 1 / stats.attackSpeed)
            {
                meleeTimer = 0;
                hasMeleed = false;
            }
            else
            {
                meleeTimer += 1.0f * Time.deltaTime;
            }
        }
        if (hasRanged)
        {
            if (rangedTimer > 1 / stats.attackSpeed)
            {
                rangedTimer = 0;
                hasRanged = false;
            }
            else
            {
                rangedTimer += 1.0f * Time.deltaTime;
            }
        }
    }
}