using UnityEngine;
using UnityEngine.AI;

// controls how the enemy moves and attacks the player
public class AiTarget : MonoBehaviour
{
    // --- variables ---
    public Transform Target; // the player or whatever this enemy is chasing
    private NavMeshAgent m_Agent; // the thing that makes the enemy walk around
    private AiStats stats; // grabbing the speed and attack numbers from the other script
    public GameObject damageBox; // the invisible box that hurts the player when it turns on
    public AudioManager audioManager;

    private Vector3 m_startingPoint; // where the enemy started so it can go back home
    [SerializeField] private float m_Distance; // how far away the player is right now

    private float currentCooldown = 0f; // timer for when it can attack again

    [Header("Combat Timing")]
    public float damageDuration = 0.05f;
    public float attackWindupTime = 2f;

    // --- setup ---
    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        stats = GetComponent<AiStats>();
        m_startingPoint = transform.position;

        // set the walking speed
        m_Agent.speed = stats.movespeed;
        // if it flies, put it higher up
        if (stats.flyHeight > 0) m_Agent.baseOffset = stats.flyHeight;

        // if we forgot to drag the player in, go find them automatically
        if (Target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) Target = playerObj.transform;
        }

        // sometimes the enemy spawns off the map, this puts them back on the floor
        if (!m_Agent.isOnNavMesh)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 5.0f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
                m_Agent.Warp(hit.position);
            }
        }

        // turn off the hurt box at the start
        if (damageBox != null) damageBox.SetActive(false);
    }

    // --- main loop ---
    void Update()
    {
        // if there is no player, do nothing
        if (Target == null) return;

        // count down the timer if we are waiting to attack again
        if (currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
        }

        // calculate distance to player
        m_Distance = Vector3.Distance(transform.position, Target.position);

        // check if player is too far away
        if (m_Distance > stats.DetectDistance)
        {
            // go back to where we started
            m_Agent.isStopped = false;
            m_Agent.destination = m_startingPoint;
        }
        else
        {
            // check if we are close enough to hit them
            if (m_Distance <= stats.AttackDistance)
            {
                // stop moving and look at the player
                m_Agent.isStopped = true;
                transform.LookAt(new Vector3(Target.position.x, transform.position.y, Target.position.z));

                // check if we are ready to attack
                if (currentCooldown <= 0)
                {
                    currentCooldown = stats.attspeed;

                    // wait a bit (windup) then run the attack function
                    Invoke("Attack", attackWindupTime);
                }
            }
            else
            {
                // chase the player if we see them but can't reach yet
                m_Agent.isStopped = false;
                m_Agent.destination = Target.position;
            }
        }
    }

    // --- attacking ---
    void Attack()
    {
        // turn on the damage box so it hurts
        if (damageBox != null) damageBox.SetActive(true);
        audioManager.PlaySFX(audioManager.explode);
        // turn it off after a tiny bit so it doesn't stay on forever
        Invoke("DisableDamageBox", damageDuration);
    }

    void DisableDamageBox()
    {
        if (damageBox != null) damageBox.SetActive(false);
    }
}