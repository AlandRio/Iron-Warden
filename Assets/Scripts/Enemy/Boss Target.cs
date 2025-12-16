using UnityEngine;
using UnityEngine.AI;

// controls the boss movement, lasers, and attacks
public class BossTarget : MonoBehaviour
{
    // --- main variables ---
    public Transform Target; // the player
    private NavMeshAgent m_Agent; // the thing that moves the boss
    private BossStats stats;
    public GameObject shieldCanvas; // the shield icon above his head

    [Header("Components")]
    public GameObject damageBox; // the hitbox that hurts the player
    public GameObject vulnerableHitbox; // the hitbox where the player can hurt the boss
    public Animator animator;
    public AudioManager audioManager;

    [Header("Environment Objects")]
    public GameObject[] lasers; // the laser traps in the room
    public GameObject[] warnings; // the warning signs before lasers fire
    public GameObject platform; // the platform that appears later

    private Vector3 m_startingPoint;
    [SerializeField] private float m_Distance; // how far away the player is

    [Header("State Flags")]
    public bool isShielded = true;
    private bool useOverhead = false; // toggles between two attack animations
    public bool firstShield = true;
    private bool isPlatformTimerActive = false;

    // --- timers & states ---
    private float attackCooldownTimer = 0f;
    private float attackWindupTimer = 0f;
    private float damageBoxTimer = 0f;
    private bool isWindingUp = false; // is he getting ready to swing?
    private bool isDamageBoxActive = false; // is the hurt box on?

    private bool isSpinning = false;
    private float timeAwayFromPlayer = 0f;
    private float currentSpinDuration = 0f;
    private float originalSpeed;
    private float originalDamage;

    private float laserTimer = 0f;
    private int laserState = 0; // 0 = wait, 1 = warn, 2 = fire
    private int currentLaserIndex = -1;
    private int lastLaserIndex = -1;

    private float platformSurvivalTimer = 0f;

    [Header("Settings")]
    public float damageDuration = 0.05f;
    public float attackWindupTime = 0.5f;
    public float walkStartDistance = 10f;
    public float spinTriggerTime = 5f;
    public float spinDuration = 2f;
    public float platformActivationTime = 30f;
    public float hysteresisBuffer = 2.0f; // prevents jittering between walk and run

    [Tooltip("How fast the boss turns to face the player. Higher = More accurate.")]
    public float turnSpeed = 10f;

    public float laserInterval = 15f;
    public float laserWarningTime = 3f;
    public float laserFireTime = 1f;

    // --- setup ---
    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        stats = GetComponent<BossStats>();
        m_startingPoint = transform.position;

        m_Agent.speed = stats.movespeed;
        originalSpeed = stats.movespeed;
        originalDamage = stats.att;

        // turn off auto rotation so we can rotate him manually later
        m_Agent.updateRotation = false;
        m_Agent.acceleration = 20f;

        if (stats.flyHeight > 0) m_Agent.baseOffset = stats.flyHeight;

        // find the player if we didn't drag them in
        if (Target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) Target = playerObj.transform;
        }

        // put him on the ground if he spawns in the air
        if (!m_Agent.isOnNavMesh)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 5.0f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
                m_Agent.Warp(hit.position);
            }
        }

        // hide all the hazards at the start
        if (damageBox != null) damageBox.SetActive(false);
        if (vulnerableHitbox != null) vulnerableHitbox.SetActive(false);
        foreach (var l in lasers) if (l) l.SetActive(false);
        foreach (var w in warnings) if (w) w.SetActive(false);
        if (platform != null) platform.SetActive(false);
    }

    // --- main loop ---
    void Update()
    {
        if (Target == null) return;

        // check how far the player is
        m_Distance = Vector3.Distance(transform.position, Target.position);

        // run all the different logic systems
        HandleShieldLogic();
        HandlePlatformLogic();
        HandleLaserLogic();
        HandleAttackTimers();

        // decide if we are spinning or chasing
        if (isSpinning)
        {
            PerformSpinMovement();
        }
        else
        {
            CheckForSpinTrigger();
            if (isSpinning) return;

            // if player is super far, go back to start
            if (m_Distance > stats.DetectDistance)
            {
                m_Agent.isStopped = false;
                m_Agent.destination = m_startingPoint;
                UpdateAnim(true, false, false);

                // face the direction we are walking
                if (m_Agent.velocity.sqrMagnitude > 0.1f)
                    transform.rotation = Quaternion.LookRotation(m_Agent.velocity.normalized);
            }
            else
            {
                // check if we are close enough to attack
                float currentAttackThreshold = stats.AttackDistance;
                if (m_Agent.isStopped) currentAttackThreshold += 1.0f;

                if (m_Distance <= currentAttackThreshold)
                {
                    PerformCombatStance();
                }
                else
                {
                    PerformChase();
                }
            }
        }
    }

    // ================= logic blocks =================

    // stops moving and gets ready to hit
    void PerformCombatStance()
    {
        m_Agent.isStopped = true;
        UpdateAnim(false, false, false);

        // look at the player unless we are already swinging
        if (!isWindingUp && !isDamageBoxActive)
        {
            FaceTarget();
        }

        // handle the attack cooldown
        if (!isWindingUp && !isDamageBoxActive)
        {
            if (attackCooldownTimer > 0)
            {
                attackCooldownTimer -= Time.deltaTime;
            }
            else
            {
                StartAttackWindup();
            }
        }
    }

    // runs towards the player
    void PerformChase()
    {
        m_Agent.isStopped = false;
        m_Agent.destination = Target.position;

        FaceTarget();

        // decide if we should walk or run based on distance
        bool isWalking = animator.GetBool("Walk");
        bool shouldWalk;

        if (isWalking) shouldWalk = m_Distance < (walkStartDistance + hysteresisBuffer);
        else shouldWalk = m_Distance < walkStartDistance;

        UpdateAnim(shouldWalk, !shouldWalk, false);
    }

    // manually rotate the boss to look at the player
    void FaceTarget()
    {
        if (Target == null) return;

        Vector3 direction = (Target.position - transform.position).normalized;
        direction.y = 0; // keep him upright

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
        }
    }

    // checks if the player is running away (triggers spin attack)
    void CheckForSpinTrigger()
    {
        if (m_Distance > stats.AttackDistance)
        {
            timeAwayFromPlayer += Time.deltaTime;
            if (timeAwayFromPlayer >= spinTriggerTime)
            {
                StartSpin();
            }
        }
        else
        {
            timeAwayFromPlayer = 0f;
        }
    }

    // start the spin attack
    void StartSpin()
    {
        isSpinning = true;
        currentSpinDuration = 0f;
        m_Agent.speed = originalSpeed * 2.0f; // go fast
        stats.att = originalDamage / 2.0f; // do less damage per hit

        if (damageBox != null) damageBox.SetActive(true);
        UpdateAnim(false, false, true);
    }

    // moving while spinning
    void PerformSpinMovement()
    {
        m_Agent.isStopped = false;
        m_Agent.destination = Target.position;

        FaceTarget();

        currentSpinDuration += Time.deltaTime;

        if (currentSpinDuration >= spinDuration)
        {
            EndSpin();
        }
    }

    // stop spinning and go back to normal
    void EndSpin()
    {
        isSpinning = false;
        timeAwayFromPlayer = 0f;
        m_Agent.speed = originalSpeed;
        stats.att = originalDamage;

        if (damageBox != null) damageBox.SetActive(false);
        UpdateAnim(false, false, false);
    }

    // ================= attack handling =================

    // prepare the attack animation
    void StartAttackWindup()
    {
        if (animator != null)
        {
            // switch between overhead and sweep attacks
            if (useOverhead) animator.SetTrigger("overhead");
            else animator.SetTrigger("sweep");

            useOverhead = !useOverhead;
        }

        ExecuteAttack();

        attackCooldownTimer = stats.attspeed;
        isWindingUp = true;
        attackWindupTimer = attackWindupTime;
    }

    // counts down the timers for the attack hitboxes
    void HandleAttackTimers()
    {
        if (isWindingUp)
        {
            attackWindupTimer -= Time.deltaTime;
            if (attackWindupTimer <= 0)
            {
                isWindingUp = false;
            }
        }

        if (isDamageBoxActive)
        {
            damageBoxTimer -= Time.deltaTime;
            if (damageBoxTimer <= 0)
            {
                if (damageBox != null) damageBox.SetActive(false);
                isDamageBoxActive = false;
            }
        }
    }

    // actually turn on the damage
    void ExecuteAttack()
    {
        if (audioManager) audioManager.PlaySFX(audioManager.explode);

        if (damageBox != null) damageBox.SetActive(true);
        isDamageBoxActive = true;
        damageBoxTimer = damageDuration;
    }

    // ================= other systems =================

    // handles the laser traps (waiting -> warning -> firing)
    void HandleLaserLogic()
    {
        laserTimer += Time.deltaTime;

        if (laserState == 0) // waiting
        {
            if (laserTimer >= laserInterval)
            {
                PickLaserIndex();
                if (warnings[currentLaserIndex] != null) warnings[currentLaserIndex].SetActive(true);
                laserState = 1;
                laserTimer = 0f;
            }
        }
        else if (laserState == 1) // warning
        {
            if (laserTimer >= laserWarningTime)
            {
                // turn off warning
                if (warnings[currentLaserIndex] != null) warnings[currentLaserIndex].SetActive(false);

                // turn on laser
                if (lasers[currentLaserIndex] != null)
                {
                    lasers[currentLaserIndex].SetActive(true);

                    // play sound
                    if (audioManager) audioManager.PlaySFX(audioManager.explode);
                }

                laserState = 2;
                laserTimer = 0f;
            }
        }
        else if (laserState == 2) // firing
        {
            if (laserTimer >= laserFireTime)
            {
                if (lasers[currentLaserIndex] != null) lasers[currentLaserIndex].SetActive(false);
                laserState = 0;
                laserTimer = 0f;
            }
        }
    }

    // chooses a random laser but tries not to pick the same one twice
    void PickLaserIndex()
    {
        if (lasers.Length <= 1)
        {
            currentLaserIndex = 0;
            return;
        }
        int newIndex = lastLaserIndex;
        while (newIndex == lastLaserIndex)
        {
            newIndex = Random.Range(0, lasers.Length);
        }
        currentLaserIndex = newIndex;
        lastLaserIndex = newIndex;
    }

    // turns on the platform after enough time has passed
    void HandlePlatformLogic()
    {
        if (!isPlatformTimerActive) return;

        if (m_Distance <= stats.DetectDistance)
        {
            platformSurvivalTimer += Time.deltaTime;
            if (platformSurvivalTimer >= platformActivationTime)
            {
                if (platform != null) platform.SetActive(true);
            }
        }
    }

    // turns the shield icon/hitbox on and off
    void HandleShieldLogic()
    {
        BossDamage bossDamage = GetComponent<BossDamage>();
        if (bossDamage != null)
        {
            if (!isShielded)
            {
                if (vulnerableHitbox != null) vulnerableHitbox.SetActive(true);
                shieldCanvas.SetActive(false);
            }
            else
            {
                if (vulnerableHitbox != null) vulnerableHitbox.SetActive(false);
                shieldCanvas.SetActive(true);
            }
        }
    }

    // helper to set all the animator bools at once
    void UpdateAnim(bool walk, bool run, bool spin)
    {
        if (animator == null) return;
        animator.SetBool("Walk", walk);
        animator.SetBool("run", run);
        animator.SetBool("spin", spin);
    }

    // call this when the shield breaks
    public void DestroyShield()
    {
        isShielded = false;
    }

    // call this to start the platform timer
    public void setPlatformTimerActive()
    {
        isPlatformTimerActive = true;
    }
}