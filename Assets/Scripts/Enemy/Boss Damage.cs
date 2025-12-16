using TMPro;
using UnityEngine;
using UnityEngine.AI; // needed for the pathfinding

// controls the boss health and what happens when he gets hurt
public class BossDamage : MonoBehaviour
{
    private BossStats stats;
    public float currentHealth;
    [SerializeField] private bossStatus healthBar;

    [SerializeField] private GameObject ChoiceCube;
    [SerializeField] private Objectives objective;
    public AudioManager audioManager;
    public bool dead;

    private bool isStopped = false; // check this so we don't try to stop him twice

    // setting things up
    void Start()
    {
        stats = GetComponent<BossStats>();
        currentHealth = stats.hp;
        healthBar.updateHealthBar(currentHealth, stats.hp);
    }

    // function for when the boss takes a hit
    public void takeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.updateHealthBar(currentHealth, stats.hp);
        Debug.Log("I took " + damage + " damage and now my hp is " + currentHealth);

        // check if he is getting weak (under 20 hp) and hasn't stopped yet
        if (currentHealth <= 20 && !isStopped)
        {
            StopWarden();
        }

        // check if he ran out of health
        if (currentHealth <= 0)
        {
            if (!dead) die();
        }
    }

    // this freezes the boss so he stops attacking
    private void StopWarden()
    {
        isStopped = true;

        // turn off the script that controls his brain so he stops thinking
        BossTarget aiScript = GetComponent<BossTarget>();
        if (aiScript != null)
        {
            aiScript.enabled = false;
        }

        // tell the pathfinding agent to stop moving physically
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            agent.ResetPath(); // forget where he was going
        }

        // force the animator to stop running or walking
        Animator anim = GetComponentInChildren<Animator>();
        if (anim != null)
        {
            anim.SetBool("Walk", false);
            anim.SetBool("run", false);
            anim.SetBool("spin", false);
            // could play a tired animation here if you wanted
        }
    }

    // what happens when he actually dies
    private void die()
    {
        // just in case he died in one hit, make sure he stops moving
        dead = true;
        if (!isStopped) StopWarden();

        Debug.Log("I died");
        // tell the game we finished the objective
        objective.completeObjective(3);
        Animator anim = GetComponentInChildren<Animator>();
        if (anim != null) anim.SetTrigger("die");
        // show the ending choice object
        ChoiceCube.SetActive(true);
    }
}