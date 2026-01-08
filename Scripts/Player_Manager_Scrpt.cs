using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Movement_Script : MonoBehaviour
{
    [Header("Movement")]
    public float groundDrag;
    public float jumpForce;
    public float smoothTime;
    public Transform orientation;

    public float airMultiplier;
    bool readyToJump;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Jump Logic")]
    public KeyCode jumpKey = KeyCode.Space;
    private float lastJumpTime;
    public float jumpCooldown = 3f;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    [Header("References")]
    public Transform playerObj;
    public Transform cam;
    public Camera mainCamera;

    [Header("Settings")]
    public float movementSpeed = 5f;
    public float sprintMultiplier = 1.5f;
    public float rotationSpeed = 10f;
    public float normalFOV = 60f;
    public float sprintFOV = 80f;
    public float fovTransitionSpeed = 5f;

    public Vector3 cameraOffset = new Vector3(0, 2f, -4f);
    public Vector3 sprintCameraOffset = new Vector3(0, 5f, -2f);

    public float cameraShiftAmount = 1.5f;
    public float positionSmoothSpeed = 5f;

    private Vector3 defaultCameraPosition;
    private Vector3 targetCameraPosition;

    private float defaultMoveSpeed;
    private float defaultJumpHeight;

    private Hover_Logic hoverHandler;

    //[Header("Hover Settings")]
    //[SerializeField] private InputActionReference movementControl;
    //[SerializeField] private InputActionReference jumpControl;
    //[SerializeField] private InputActionReference sprintControl;

    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;

    [Header("Hoverboard Settings")]
    [SerializeField] private float hoverHeight = 1.5f;
    [SerializeField] private float hoverboardSpeed = 15f;
    [SerializeField] private float hoverboardTurnSpeed = 2.5f;
    [SerializeField] private float hoverboardAcceleration = 2.0f;
    [SerializeField] private float hoverboardDeceleration = 1.0f;
    [SerializeField] private float hoverGravity = -2.0f;
    [SerializeField] private float hoverDampening = 0.95f;
    [SerializeField] private float groundRaycastDistance = 10f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float hoverboardTrickMultiplier = 2.0f;
    [SerializeField] private GameObject hoverboardVisual;
    [SerializeField] private ParticleSystem hoverParticles;

    [Header("Interact Settings")]
    [SerializeField] private GameObject interactionPromptUI;
    [SerializeField] private float interactRange = 2f;

    private CharacterController controller;
    private Hover_Logic hoverboardController;
    private Player_Attack homingAttack;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;

        defaultCameraPosition = cameraOffset;
        targetCameraPosition = cameraOffset;

        mainCamera.fieldOfView = normalFOV;

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        defaultMoveSpeed = movementSpeed;
        defaultJumpHeight = jumpForce;

        homingAttack = GetComponent<Player_Attack>();



        controller = GetComponent<CharacterController>();

        hoverboardController = new Hover_Logic(
            transform,
            controller,
            playerSpeed,
            sprintMultiplier,
            jumpHeight,
            gravityValue,
            hoverHeight,
            hoverboardSpeed,
            hoverboardTurnSpeed,
            hoverboardAcceleration,
            hoverboardDeceleration,
            hoverGravity,
            hoverDampening,
            groundRaycastDistance,
            groundLayer,
            hoverboardTrickMultiplier,
            hoverboardVisual,
            hoverParticles,
            interactionPromptUI,
            interactRange
        );
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        hoverboardController.Update();
        MyInput();
        SpeedControl();

        rb.drag = grounded ? groundDrag : 0;

        targetCameraPosition = cameraOffset;

        // Adjust camera for sprint mode
        if (Input.GetKey(KeyCode.LeftShift))
        {
            targetCameraPosition = sprintCameraOffset;
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, sprintFOV, Time.deltaTime * fovTransitionSpeed);
        }
        else
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, normalFOV, Time.deltaTime * fovTransitionSpeed);
        }

        cam.position = Vector3.Lerp(cam.position, playerObj.position + targetCameraPosition, Time.deltaTime * positionSmoothSpeed);

        Vector3 cameraForward = cam.forward;
        cameraForward.y = 0;
        playerObj.forward = Vector3.Slerp(playerObj.forward, cameraForward.normalized, Time.deltaTime * rotationSpeed);

        Vector3 moveDirection = cam.forward * verticalInput + cam.right * horizontalInput;
        moveDirection.y = 0;

        float currentSpeed = (Input.GetKey(KeyCode.LeftShift)) ? movementSpeed * sprintMultiplier : movementSpeed;
        rb.velocity = new Vector3(moveDirection.x * currentSpeed, rb.velocity.y, moveDirection.z * currentSpeed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "SpeedBoost":
                movementSpeed *= 2;
                Debug.Log($"Movementspeed has been increased to {movementSpeed}");
                break;
            case "JumpPad":
                jumpForce = 25f;
                readyToJump = true;
                Debug.Log($"");
                break;
        }
        if (collision.gameObject.tag == "Ground")
        {
            movementSpeed = defaultMoveSpeed;
            jumpForce = defaultJumpHeight;
            readyToJump = true;
            Debug.Log($"Jump force has been set to {jumpForce} and defaultJumpHeight has been set to {defaultJumpHeight} XxXxX");
        }
        else if (collision.gameObject.tag != "Ground" && collision.gameObject.tag != "JumpPad" && collision.gameObject.tag != "SpeedBoost")
        {
            Debug.Log($"Jump force has been set to {jumpForce} and defaultJumpHeight has been set to {defaultJumpHeight}");
            jumpForce = 0;
            readyToJump = false;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
        KeepPlayerGrounded();
    }

    private void MyInput()
    {
        if (Input.GetKey(jumpKey) && readyToJump)
        {
            Debug.Log($"Jump has been detected by pressing {jumpKey}");
            readyToJump = false;

            Jump();
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (grounded)
            rb.AddForce(moveDirection.normalized * movementSpeed * 10f, ForceMode.Force);
        else
            rb.AddForce(moveDirection.normalized * movementSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > movementSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * movementSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void KeepPlayerGrounded()
    {
        if (!grounded)
        {
            rb.AddForce(Vector3.down * 30f, ForceMode.Acceleration);
        }
    }
}
