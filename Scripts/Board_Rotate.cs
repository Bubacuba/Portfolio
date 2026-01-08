using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.XR;

public class Board_Rotate : MonoBehaviour
{
    [SerializeField]
    private InputActionReference movementControl;
    [SerializeField]
    private InputActionReference jumpControl;
    [SerializeField]
    private InputActionReference sprintControl;
    [SerializeField]
    private InputActionReference trickControl;
    [SerializeField]
    private InputActionReference attackControl;
    [SerializeField]
    private InputActionReference hoverboardToggleControl;
    [SerializeField]
    private InputActionReference crouchControl;
    [SerializeField]
    private InputActionReference interactControl;
    [SerializeField]
    private InputActionReference inventoryToggleControl;
    [SerializeField]
    private InputActionReference spinAttackControl;


    [Header("Inventory Settings")]
    [SerializeField] private GameObject inventoryUI; // UI panel for inventory
    //[SerializeField] private GameObject inventoryItemPrefab; // Prefab for each inventory item (optional, for cleaner UI)
    private List<Item> inventory = new List<Item>(); // Store items
    private bool isInventoryOpen = false;

    [Header("Movement Settings")]
    [SerializeField]
    private float playerSpeed = 2.0f;
    [SerializeField]
    private float sprintMultiplier = 1.5f;
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float gravityValue = -9.81f;
    [SerializeField]
    private float rotationSpeed = 4f;

    [Header("Crouch Settings")]
    [SerializeField]
    private float crouchSpeedMultiplier = 0.5f;
    [SerializeField]
    private float crouchHeight = 0.5f;
    private float standingHeight;
    private bool isCrouching = false;

    [Header("Attack Settings")]
    [SerializeField] private float homingRange = 10f;
    [SerializeField] private float attackSpeed = 20f;
    [SerializeField] private float attackDuration = 0.3f;
    [SerializeField] private GameObject attackPromptUI; // Drag UI prompt prefab here

    [Header("Spin Attack Settings")]
    [SerializeField] private float spinAttackRadius = 5f; // Radius of the AoE
    [SerializeField] private float spinAttackDamage = 50f; // Damage dealt to enemies
    [SerializeField] private float spinAttackDuration = 0.5f; // Duration of the attack
    [SerializeField] private float spinAttackCooldown = 1f; // Cooldown to prevent spamming
    [SerializeField] private GameObject spinAttackEffect; // Optional visual effect (e.g., particle system)
    private bool canSpinAttack = true; // Cooldown tracker for spin attack
    private bool isSpinAttacking = false; // Track spin attack separately from homing attack

    [Header("Hoverboard Settings")]
    [SerializeField] private float hoverHeight = 1.5f; // Height above ground when hovering
    [SerializeField] private float hoverboardSpeed = 15f; // Speed when on hoverboard
    [SerializeField] private float hoverboardTurnSpeed = 2.5f; // Turning responsiveness
    [SerializeField] private float hoverboardAcceleration = 2.0f; // How quickly the board speeds up
    [SerializeField] private float hoverboardDeceleration = 1.0f; // How quickly the board slows down
    [SerializeField] private float hoverGravity = -2.0f; // Reduced gravity when hovering
    [SerializeField] private float hoverDampening = 0.95f; // Dampening of velocity while hovering
    [SerializeField] private float groundRaycastDistance = 10f; // How far to check for ground
    [SerializeField] private LayerMask groundLayer; // Which layers count as ground
    [SerializeField] private float hoverboardTrickMultiplier = 2.0f; // Score multiplier for tricks on hoverboard
    [SerializeField] private GameObject hoverboardVisual; // The visual model of the hoverboard
    [SerializeField] private ParticleSystem hoverParticles; // Optional particles for hover effect

    private IInteractable nearbyInteractable = null;
    [Header("Interact Settings")]
    [SerializeField] private float interactRange = 2f; // Range to detect interactable objects
    [SerializeField] private GameObject interactionPromptUI; // UI to show interaction prompt

    private Transform currentTarget;
    private bool isAttacking = false;
    private bool isHoverboardActive = false;
    private float currentHoverboardSpeed = 0f;
    private Vector3 hoverboardVelocity;
    private float distanceFromGround;
    private Vector3 groundNormal = Vector3.up;

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Transform cameraMainTransform;

    private float defaultMoveSpeed;
    private float defaultJumpHeight;

    private bool canDoubleJump = false;
    private bool hasDoubleJumped = false;
    private RaycastHit groundHit;

    private MovingPlatform currentPlatform = null; // Track the current moving platform
    private Vector3 lastPlatformPosition; // Track last position for velocity calculation

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHoverboard();
    }

    private void ApplyHoverPhysics()
    {
        // Apply side friction when turning to simulate sliding
        if (Mathf.Abs(currentHoverboardSpeed) > 1.0f)
        {
            // Calculate slide factor based on turn input
            float slideAmount = Mathf.Abs(movementControl.action.ReadValue<Vector2>().x) * 0.5f;

            // Apply some side velocity based on turning
            Vector3 rightDir = transform.right * movementControl.action.ReadValue<Vector2>().x * slideAmount;
            controller.Move(rightDir * Time.deltaTime * currentHoverboardSpeed * 0.3f);
        }
    }

    private void CheckGroundDistance()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out groundHit, groundRaycastDistance, groundLayer))
        {
            distanceFromGround = groundHit.distance;
            groundNormal = groundHit.normal;

            // Optional: Align to ground surface
            if (groundNormal != Vector3.up)
            {
                Quaternion targetRotation = Quaternion.FromToRotation(transform.up, groundNormal) * transform.rotation;
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 2.0f);
            }
        }
        else
        {
            distanceFromGround = groundRaycastDistance;
            groundNormal = Vector3.up; // Reset to upright when no ground
        }
    }

    private void UpdateHoverboard()
    {
        // Check if we're above ground and get distance
        CheckGroundDistance();

        if (distanceFromGround >= groundRaycastDistance) // No ground detected
        {
            // Reset rotation to upright if no ground is found
            transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f); // Keep Y rotation, reset X and Z
            hoverboardVelocity = Vector3.zero; // Reset velocity
            currentHoverboardSpeed = 0f; // Stop movement
            return; // Skip further hoverboard updates
        }

        // Handle hover physics
        ApplyHoverPhysics();

        // Handle hoverboard movement
        Vector2 input = movementControl.action.ReadValue<Vector2>();
        float sprintInput = sprintControl.action.ReadValue<float>(); // Sprint input

        // Base speed (without sprint)
        float baseSpeed = hoverboardSpeed;

        // Apply sprint multiplier if sprinting
        float currentMaxSpeed = sprintInput > 0 ? baseSpeed * sprintMultiplier : baseSpeed;

        if (Mathf.Abs(currentHoverboardSpeed) <= currentMaxSpeed)
        {
            if (input.y > 0.1f)
            {
                currentHoverboardSpeed += hoverboardAcceleration * Time.deltaTime;
            }
            else if (input.y < -0.1f)
            {
                currentHoverboardSpeed -= hoverboardAcceleration * Time.deltaTime;
            }
            else
            {
                // Gradually slow down when no input
                currentHoverboardSpeed *= (1.0f - hoverboardDeceleration * Time.deltaTime);
            }

            // Clamp speed to the current max speed (with or without sprint)
            currentHoverboardSpeed = Mathf.Clamp(currentHoverboardSpeed, -currentMaxSpeed, currentMaxSpeed);
        }

        // Acceleration based on input
        if (input.y > 0.1f)
        {
            currentHoverboardSpeed += hoverboardAcceleration * Time.deltaTime;
        }
        else if (input.y < -0.1f)
        {
            currentHoverboardSpeed -= hoverboardAcceleration * Time.deltaTime;
        }
        else
        {
            // Gradually slow down when no input
            currentHoverboardSpeed *= (1.0f - hoverboardDeceleration * Time.deltaTime);
        }

        // Clamp speed to the current max speed (with or without sprint)
        currentHoverboardSpeed = Mathf.Clamp(currentHoverboardSpeed, -currentMaxSpeed, currentMaxSpeed);

        // Calculate movement vector
        Vector3 forwardMovement = transform.forward * currentHoverboardSpeed;

        // Apply turning based on input.x
        float turn = input.x * hoverboardTurnSpeed;
        transform.Rotate(0, turn, 0);

        // Combine movement
        Vector3 horizontalMovement = forwardMovement;

        // Apply gravity and height correction
        if (distanceFromGround > hoverHeight)
        {
            hoverboardVelocity.y += gravityValue * Time.deltaTime; // Use standard gravity
        }
        else if (distanceFromGround < hoverHeight)
        {
            // Stronger push upward if too close to ground
            float correctionForce = (hoverHeight - distanceFromGround) * 10.0f; // Increased correction force
            hoverboardVelocity.y = Mathf.Lerp(hoverboardVelocity.y, correctionForce, Time.deltaTime * 10.0f);
        }
        else
        {
            // If at ideal height, minimal dampening to maintain position
            hoverboardVelocity.y *= 0.9f; // Reduced dampening for quicker stabilization
        }

        // Handle jump (like normal movement)
        bool isGrounded = distanceFromGround < hoverHeight * 1.5f;

        if (jumpControl.action.triggered && isGrounded)
        {
            // Use the same jump calculation as normal movement
            hoverboardVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            Debug.Log("Hoverboard Jump!");
        }

        // Combine all movement
        Vector3 movement = horizontalMovement + new Vector3(0, hoverboardVelocity.y, 0);

        // Apply movement
        controller.Move(movement * Time.deltaTime);

        // Handle tricks only when high enough above ground
        float minTrickHeight = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue) * 0.8f; // Adjust this multiplier as needed

        //if (trickControl.action.triggered && distanceFromGround > minTrickHeight)
        //{
        //    DoTrick(hoverboardTrickMultiplier);
        //    Debug.Log($"Trick performed! Height: {distanceFromGround:F1} (min required: {minTrickHeight:F1})");
        //}

        // Update visual effects
        if (hoverParticles != null)
        {
            var emission = hoverParticles.emission;
            emission.rateOverTime = Mathf.Abs(currentHoverboardSpeed) * 5;
        }

        // Align with ground normal if on surface
        if (distanceFromGround < groundRaycastDistance)
        {
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, groundNormal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5.0f); // Smooth alignment
        }

        //Debug.Log($"Hoverboard Height: {transform.position.y}, Distance from Ground: {distanceFromGround}, Velocity Y: {hoverboardVelocity.y}");
    }
}
