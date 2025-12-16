using UnityEngine;
using UnityEngine.InputSystem;

// big script that handles running, jumping, climbing and dashing
public class PlayerMovement : MonoBehaviour
{
    [Header("Status Bar")]
    [SerializeField] private playerStatus statusBar;
    [Header("Climbing Settings")]
    public float climbSpeed = 5f;
    public float detectionRange = 2f; // how close to the wall we need to be
    public LayerMask climbableLayer;
    [SerializeField] private bool isClimbing = false;

    // cooldown timer to prevent re-grabbing wall immediately after jumping off
    private float climbTimer = 0f;

    [Header("Physics Settings")]
    public float gravity = -30f;
    public float fallMultiplier = 2f; // makes falling feel heavier

    [Header("Jump Settings")]
    public float coyoteTimeDuration = 0.2f; // how long you can jump after falling off
    private float coyoteTimeCounter;        // tracks the time remaining

    [Header("General Stats")]
    private bool isSprinting = false;
    private PlayerStats stats;
    private PlayerUpgrades upgrades;
    private CharacterController controller;
    private Vector2 moveInput;
    private Vector3 velocity;
    [SerializeField] private Transform cameraTransform;
    private bool faceDirection = true;

    // jump & dash states
    private bool doubleJumpUsed = false;
    private bool dashAirUsed = false;
    private bool dashUsed = false;
    private float dashTimer = 0f;
    private float jumpTimer = 0f;

    // audio & footsteps
    private float footstepTimer = 0f;
    public float footstepSpeed = 0.4f;
    public AudioManager audioManager;

    // animation
    private Animator animator;

    // setup everything
    void Start()
    {
        controller = GetComponent<CharacterController>();
        stats = GetComponent<PlayerStats>();
        upgrades = GetComponent<PlayerUpgrades>();
        animator = GetComponentInChildren<Animator>();
        if (upgrades.canDash) statusBar.updateDashText(0, 0, true, true);
        else statusBar.updateDashText(0, 0, false, false);

        if (climbableLayer == 0)
        {
            climbableLayer = LayerMask.GetMask("Default");
            Debug.LogWarning("Climbable Layer was 'Nothing'. Defaulting to 'Default' layer.");
        }
    }

    // reads the input from the controller/keyboard
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    // handles pressing the space bar
    public void OnJump(InputAction.CallbackContext context)
    {
        // 1. climbing jump (jump off the wall)
        if (isClimbing)
        {
            if (context.performed)
            {
                isClimbing = false;
                climbTimer = 0.5f;
                velocity.y = Mathf.Sqrt(stats.jumpForce * -2.0f * gravity);
                jumpTimer = 0.2f;

                // reset coyote time so we don't accidentally double jump immediately
                coyoteTimeCounter = 0f;

                Vector3 pushDir = -transform.forward * 5f;
                controller.Move(pushDir * Time.deltaTime);

                if (animator != null)
                {
                    animator.SetBool("isClimbing", false);
                    animator.SetTrigger("Jump");
                }
            }
            return;
        }

        // 2. standard & double jump
        if (context.performed)
        {
            // --- modified logic start ---
            // instead of checking isGrounded, we check if we have coyote time left
            if (coyoteTimeCounter > 0f)
            {
                audioManager.PlaySFX(audioManager.click);
                velocity.y = Mathf.Sqrt(stats.jumpForce * -2.0f * gravity);

                jumpTimer = 0.2f;
                coyoteTimeCounter = 0f; // important: consume coyote time immediately so we don't jump again

                if (animator != null) animator.SetTrigger("Jump");
            }
            // double jump logic
            // we check if coyote time is 0 (meaning we are truly in the air)
            else if (upgrades.canDoubleJump && !doubleJumpUsed && jumpTimer <= 0)
            {
                if (!doubleJumpUsed)
                {
                    audioManager.PlaySFX(audioManager.click);
                    velocity.y = Mathf.Sqrt(stats.jumpForce * -2.0f * gravity);
                    doubleJumpUsed = true;
                    if (animator != null) animator.SetTrigger("Jump");
                }
            }
            // --- modified logic end ---
        }
    }

    // toggles running
    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed && upgrades.canSprint && !isSprinting)
        {
            isSprinting = true;
            statusBar.changeSprintText("^\n^\n");
        }
        else if (context.performed && upgrades.canSprint && isSprinting)
        {
            isSprinting = false;
            statusBar.changeSprintText("^");
        }
    }

    // handles the dash ability
    public void OnDash(InputAction.CallbackContext context)
    {
        if (isClimbing) return;

        if (context.performed && upgrades.canDash && !dashAirUsed && !dashUsed)
        {
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;
            forward.y = 0; right.y = 0;
            forward.Normalize(); right.Normalize();

            // calculate where to dash
            Vector3 dashDirection = forward * moveInput.y + right * moveInput.x;
            if (dashDirection == Vector3.zero) dashDirection = transform.forward;

            if (animator != null) animator.SetTrigger("Dash");

            velocity.y = 0;
            controller.Move(dashDirection.normalized * (stats.moveSpeed * 3.0f));

            if (!controller.isGrounded) dashAirUsed = true;
            dashUsed = true;

            // dashing should probably consume coyote time to prevent jump-after-dash exploits
            coyoteTimeCounter = 0f;
        }
    }

    // main loop
    void Update()
    {
        // update timers
        if (jumpTimer > 0) jumpTimer -= Time.deltaTime;
        if (climbTimer > 0) climbTimer -= Time.deltaTime;

        CheckForClimb();

        if (isClimbing)
        {
            coyoteTimeCounter = 0f; // no coyote time while climbing
            PerformClimbingMovement();
        }
        else
        {
            HandleCoyoteTime(); // update the timer
            PerformStandardMovement();
        }

        HandleDashTimer();
    }

    // helps us jump nicely even if we walked off the edge a tiny bit
    void HandleCoyoteTime()
    {
        if (controller.isGrounded)
        {
            // if we are on the ground, keep the timer full
            coyoteTimeCounter = coyoteTimeDuration;
        }
        else
        {
            // if we are in the air, drain the timer
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    // ================= climbing logic =================
    // checks if there is a wall in front of us
    void CheckForClimb()
    {
        if (climbTimer > 0) return;

        RaycastHit hit;
        Vector3 headRay = transform.position + Vector3.up * 1.7f;
        float actualRange = isClimbing ? detectionRange * 1.2f : detectionRange;
        bool headHit = Physics.Raycast(headRay, transform.forward, out hit, actualRange, climbableLayer);

        if (headHit && hit.collider.CompareTag("Climbable"))
        {
            if (moveInput.y > 0.1f || isClimbing)
            {
                if (!isClimbing)
                {
                    doubleJumpUsed = false;
                    dashAirUsed = false;
                    isClimbing = true;
                }
            }
        }
        else
        {
            isClimbing = false;
        }
    }

    // moves the player up the wall
    void PerformClimbingMovement()
    {
        float verticalClimb = moveInput.y * climbSpeed;
        Vector3 climbMovement = new Vector3(0, verticalClimb, 0) + (transform.forward * 0.5f);

        controller.Move(climbMovement * Time.deltaTime);
        velocity = Vector3.zero;

        if (animator != null)
        {
            animator.SetBool("isClimbing", true);
            float animSpeed = Mathf.Abs(moveInput.y) > 0.1f ? 1f : 0f;
            animator.SetFloat("climbSpeed", animSpeed);
            animator.SetBool("isFalling", false);
        }
    }

    // ================= standard movement =================
    // moves the player on the ground
    void PerformStandardMovement()
    {
        if (animator != null) animator.SetBool("isClimbing", false);

        bool isCurrentlyGrounded = controller.isGrounded;

        // reset abilities if grounded
        if (isCurrentlyGrounded && jumpTimer <= 0)
        {
            doubleJumpUsed = false;
            dashAirUsed = false;
            if (velocity.y < 0) velocity.y = -2f;
        }

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0; right.y = 0;
        forward.Normalize(); right.Normalize();
        Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;

        float currentSpeed;
        if (isCurrentlyGrounded)
        {
            currentSpeed = isSprinting ? stats.moveSpeed * stats.sprintSpeed : stats.moveSpeed;
        }
        else
        {
            currentSpeed = stats.moveSpeed * 2f;
        }

        controller.Move(moveDirection * currentSpeed * Time.deltaTime);

        // rotate player to face where they are going
        if (faceDirection && moveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
        }

        float appliedGravity = gravity;
        if (velocity.y < 0) appliedGravity *= fallMultiplier;

        velocity.y += appliedGravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        HandleFootsteps(isCurrentlyGrounded, moveDirection);
        HandleStandardAnimations(isCurrentlyGrounded, moveDirection);
    }

    // ================= helpers =================
    // counts down until we can dash again
    void HandleDashTimer()
    {
        if (dashUsed)
        {
            statusBar.updateDashText(dashTimer, stats.dashCD, true, false);
            if (dashTimer > stats.dashCD)
            {
                statusBar.updateDashText(dashTimer, stats.dashCD, true, true);
                dashTimer = 0;
                dashUsed = false;
            }
            else
            {
                dashTimer += 1.0f * Time.deltaTime;
            }
        }
    }

    // plays footstep sounds based on where we are
    void HandleFootsteps(bool isGrounded, Vector3 moveDir)
    {
        bool isMoving = moveDir.sqrMagnitude > 0.0001f;
        float currentStepSpeed = isSprinting ? 0.25f : 0.4f;

        if (footstepTimer > 0) footstepTimer -= Time.deltaTime;

        if (isMoving && isGrounded)
        {
            if (footstepTimer <= 0)
            {
                AudioClip[] steps;
                if (upgrades.inFortress)
                {
                    steps = new AudioClip[] { audioManager.footstepFort1, audioManager.footstepFort2, audioManager.footstepFort3, audioManager.footstepFort4 };
                }
                else
                {
                    steps = new AudioClip[] { audioManager.footstepGrass1, audioManager.footstepGrass2, audioManager.footstepGrass3, audioManager.footstepGrass4 };
                }

                if (steps.Length > 0 && audioManager != null)
                {
                    int randomIndex = Random.Range(0, steps.Length);
                    audioManager.PlaySFX(steps[randomIndex]);
                }
                footstepTimer = currentStepSpeed;
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }

    // tells the animator what to do
    void HandleStandardAnimations(bool isGrounded, Vector3 moveDir)
    {
        if (animator == null) return;

        bool effectiveGroundedStatus = isGrounded && jumpTimer <= 0;
        animator.SetBool("isGrounded", effectiveGroundedStatus);

        if (!effectiveGroundedStatus)
        {
            bool isFalling = velocity.y < 0;
            animator.SetBool("isFalling", isFalling);
        }
        else
        {
            animator.SetBool("isFalling", false);
        }

        bool isMoving = moveInput.sqrMagnitude > 0.01f;

        if (effectiveGroundedStatus)
        {
            animator.SetBool("isWalking", isMoving);
            animator.SetBool("isRunning", isMoving && isSprinting);
        }
        else
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
        }
    }
}