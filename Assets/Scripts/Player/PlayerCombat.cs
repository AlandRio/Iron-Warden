using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// handles hitting enemies, taking damage, and healing
public class PlayerCombat : MonoBehaviour
{
    private Animator m_Animator;
    private PlayerStats stats;
    private PlayerInput playerInput;

    // combat variables
    private bool hasMeleed = false; // check if we are already swinging
    private float meleeTimer = 0f;
    public Collider weaponCollider; // the hitbox on the sword/fist

    // health variables
    private float currentHealth;

    // regen variables
    private float lastDamageTime;
    private float regenDelay = 6f; // wait 6 seconds
    private float regenRate = 1f;  // recover 1 hp per second

    [SerializeField] private playerStatus statusBar;
    [SerializeField] private AudioManager audioManager;

    // setup
    void Start()
    {
        m_Animator = GetComponentInChildren<Animator>();
        stats = GetComponent<PlayerStats>();
        playerInput = GetComponent<PlayerInput>();

        // set current health to max (stats.health)
        currentHealth = stats.health;

        // initialize timer so you can regen immediately if needed
        lastDamageTime = -regenDelay;
    }

    // called when we press the attack button
    public void OnMelee(InputAction.CallbackContext context)
    {
        if (playerInput != null && !playerInput.enabled) return;

        // only attack if we aren't already attacking
        if (context.performed && !hasMeleed)
        {
            m_Animator.SetTrigger("Melee");
            weaponCollider.enabled = true; // turn on the damage box
            hasMeleed = true;
            audioManager.PlaySFX(audioManager.punch);
        }
    }

    // gets called when something hurts us
    public void takeDamage(float damage)
    {
        // 1. reset the regen timer because we got hit!
        lastDamageTime = Time.time;

        if (currentHealth - damage > 0)
        {
            m_Animator.SetTrigger("Hit");
        }
        currentHealth -= damage;

        if (statusBar != null)
            statusBar.updateHealthBar(currentHealth, stats.health);

        // check if we died
        if (currentHealth <= 0)
        {
            if (!stats.dead) die();
        }
    }

    // handles dying logic
    public void die()
    {
        if (playerInput != null) playerInput.DeactivateInput(); // stop player moving
        audioManager.PlaySFX(audioManager.death);
        m_Animator.SetTrigger("Die");

        Invoke("FinishDying", 2f); // wait a bit then reset
    }

    private void FinishDying()
    {
        stats.die();
    }

    // resets everything so we can play again
    public void RespawnInput()
    {
        currentHealth = stats.health; // reset to max health
        lastDamageTime = Time.time;   // reset regen timer

        if (statusBar != null)
            statusBar.updateHealthBar(currentHealth, stats.health);

        m_Animator.Play("Idle");
        m_Animator.ResetTrigger("Die");

        if (playerInput != null) playerInput.ActivateInput();
    }

    void Update()
    {
        // --- regeneration logic start ---
        // only regen if we are alive and hurt
        if (!stats.dead && currentHealth < stats.health)
        {
            // check if 6 seconds have passed since last hit
            if (Time.time - lastDamageTime > regenDelay)
            {
                currentHealth += regenRate * Time.deltaTime;
                if (currentHealth > stats.health) currentHealth = stats.health;
                if (statusBar != null)
                    statusBar.updateHealthBar(currentHealth, stats.health);
            }
        }
        // --- regeneration logic end ---

        // melee timer logic
        if (hasMeleed)
        {
            // turn off the hitbox halfway through the swing
            if (meleeTimer > 0.5f)
            {
                weaponCollider.enabled = false;
            }
            // reset the swing so we can attack again
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
    }
}