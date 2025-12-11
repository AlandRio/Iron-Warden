using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private float gravity = -9.8f;
    private bool isSprinting = false;
    private PlayerStats stats;
    private PlayerUpgrades upgrades;
    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 velocity;
    [SerializeField] private Transform cameraTransform;
    private bool faceDirection = true;
    private bool doubleJumpUsed = false;
    private bool dashAirUsed = false;
    private bool dashUsed = false;
    private float dashTimer = 0f;

    // --- FIX 1: Add a timer to track when we last jumped ---
    private float jumpTimer = 0f;

    private Animator animator;
    private readonly int IsGroundedHash = Animator.StringToHash("isGrounded");
    private readonly int IsFallingHash = Animator.StringToHash("isFalling");
    private readonly int IsWalkingHash = Animator.StringToHash("isWalking");
    private readonly int IsRunningHash = Animator.StringToHash("isRunning");
    private readonly int IsJumpingHash = Animator.StringToHash("isJumping");
    private readonly int IsDoubleJumpingHash = Animator.StringToHash("isDoubleJumping");
    private readonly int IsDashingHash = Animator.StringToHash("isDashing");

    void Start()
    {
        controller = GetComponent<CharacterController>();
        stats = GetComponent<PlayerStats>();
        upgrades = GetComponent<PlayerUpgrades>();
        animator = GetComponentInChildren<Animator>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (controller.isGrounded)
            {
                velocity.y = Mathf.Sqrt(stats.jumpForce * -2.0f * gravity);

                jumpTimer = 0.2f;

                if (animator != null)
                {
                    animator.SetBool(IsJumpingHash, true);
                    animator.SetBool(IsDoubleJumpingHash, false);
                }
            }
            else if (!controller.isGrounded && upgrades.canDoubleJump)
            {
                if (!doubleJumpUsed)
                {
                    velocity.y = Mathf.Sqrt(stats.jumpForce * -2.0f * gravity);
                    doubleJumpUsed = true;
                    if (animator != null)
                    {
                        animator.SetBool(IsDoubleJumpingHash, true);
                    }
                }
            }
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed && upgrades.canSprint && !isSprinting) isSprinting = true;
        else if (context.performed && upgrades.canSprint && isSprinting) isSprinting = false;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && upgrades.canDash && !dashAirUsed && !dashUsed)
        {
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;
            forward.y = 0; right.y = 0;
            forward.Normalize(); right.Normalize();

            Vector3 dashDirection = forward * moveInput.y + right * moveInput.x;
            if (dashDirection == Vector3.zero) dashDirection = transform.forward;

            if (animator != null) animator.SetBool(IsDashingHash, true);

            velocity.y = 0;
            controller.Move(dashDirection.normalized * (stats.moveSpeed * 3.0f));

            if (!controller.isGrounded) dashAirUsed = true;
            dashUsed = true;
        }
    }

    void Update()
    {
        if (jumpTimer > 0)
        {
            jumpTimer -= Time.deltaTime;
        }

        bool isCurrentlyGrounded = controller.isGrounded;

        if (isCurrentlyGrounded && jumpTimer <= 0)
        {
            doubleJumpUsed = false;
            dashAirUsed = false;

            if (animator != null)
            {
                animator.SetBool(IsJumpingHash, false);
                animator.SetBool(IsDoubleJumpingHash, false);
                animator.SetBool(IsFallingHash, false);
            }

            if (velocity.y < 0) velocity.y = -2f;
        }

        if (dashUsed)
        {
            if (dashTimer >= 0.5f)
            {
                if (animator != null) animator.SetBool(IsDashingHash, false);
            }
            if (dashTimer > stats.dashCD)
            {
                dashTimer = 0;
                dashUsed = false;
            }
            else
            {
                dashTimer += 1.0f * Time.deltaTime;
            }
        }

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0; right.y = 0;
        forward.Normalize(); right.Normalize();
        Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;

        float currentSpeed = isSprinting ? stats.moveSpeed * stats.sprintSpeed : stats.moveSpeed;
        controller.Move(moveDirection * currentSpeed * Time.deltaTime);

        if (faceDirection && moveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
        }

        velocity.y += gravity * 2.0f * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        bool isMoving = moveDirection.sqrMagnitude > 0.0001f;

        if (animator != null)
        {
            bool effectiveGroundedStatus = isCurrentlyGrounded && jumpTimer <= 0;

            animator.SetBool(IsGroundedHash, effectiveGroundedStatus);

            if (!effectiveGroundedStatus)
            {
                bool isFalling = velocity.y < 0;
                animator.SetBool(IsFallingHash, isFalling);

                if (isFalling)
                {
                    animator.SetBool(IsJumpingHash, false);
                    animator.SetBool(IsDoubleJumpingHash, false);
                }
            }

            if (effectiveGroundedStatus)
            {
                animator.SetBool(IsWalkingHash, isMoving);
                animator.SetBool(IsRunningHash, isMoving && isSprinting);
            }
            else
            {
                animator.SetBool(IsWalkingHash, false);
                animator.SetBool(IsRunningHash, false);
            }
        }
    }
}