using UnityEngine;

public class Hover_Logic
{
    private Transform playerTransform;
    private CharacterController controller;

    private float playerSpeed;
    private float sprintMultiplier;
    private float jumpHeight;
    private float gravityValue;

    private float hoverHeight;
    private float hoverboardSpeed;
    private float hoverboardTurnSpeed;
    private float hoverboardAcceleration;
    private float hoverboardDeceleration;
    private float hoverGravity;
    private float hoverDampening;
    private float groundRaycastDistance;
    private LayerMask groundLayer;
    private float hoverboardTrickMultiplier;

    private GameObject hoverboardVisual;
    private ParticleSystem hoverParticles;

    private float currentHoverboardSpeed;
    private Vector3 hoverboardVelocity;
    private float distanceFromGround;
    private Vector3 groundNormal = Vector3.up;
    private RaycastHit groundHit;

    public Hover_Logic(
        Transform playerTransform,
        CharacterController controller,
        float playerSpeed,
        float sprintMultiplier,
        float jumpHeight,
        float gravityValue,
        float hoverHeight,
        float hoverboardSpeed,
        float hoverboardTurnSpeed,
        float hoverboardAcceleration,
        float hoverboardDeceleration,
        float hoverGravity,
        float hoverDampening,
        float groundRaycastDistance,
        LayerMask groundLayer,
        float hoverboardTrickMultiplier,
        GameObject hoverboardVisual,
        ParticleSystem hoverParticles,
        GameObject interactionPromptUI,
        float interactRange
    )
    {
        this.playerTransform = playerTransform;
        this.controller = controller;
        this.playerSpeed = playerSpeed;
        this.sprintMultiplier = sprintMultiplier;
        this.jumpHeight = jumpHeight;
        this.gravityValue = gravityValue;

        this.hoverHeight = hoverHeight;
        this.hoverboardSpeed = hoverboardSpeed;
        this.hoverboardTurnSpeed = hoverboardTurnSpeed;
        this.hoverboardAcceleration = hoverboardAcceleration;
        this.hoverboardDeceleration = hoverboardDeceleration;
        this.hoverGravity = hoverGravity;
        this.hoverDampening = hoverDampening;
        this.groundRaycastDistance = groundRaycastDistance;
        this.groundLayer = groundLayer;
        this.hoverboardTrickMultiplier = hoverboardTrickMultiplier;
        this.hoverboardVisual = hoverboardVisual;
        this.hoverParticles = hoverParticles;
    }

    private void CheckGroundDistance()
    {
        if (Physics.Raycast(playerTransform.position, Vector3.down, out groundHit, groundRaycastDistance, groundLayer))
        {
            distanceFromGround = groundHit.distance;
            groundNormal = groundHit.normal;

            Quaternion targetRotation = Quaternion.FromToRotation(playerTransform.up, groundNormal) * playerTransform.rotation;
            playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation, targetRotation, Time.deltaTime * 2.0f);
        }
        else
        {
            distanceFromGround = groundRaycastDistance;
            groundNormal = Vector3.up;
        }
    }

    private void ApplyHoverPhysics(Vector2 input)
    {
        if (Mathf.Abs(currentHoverboardSpeed) > 1.0f)
        {
            float slideAmount = Mathf.Abs(input.x) * 0.5f;
            Vector3 rightDir = playerTransform.right * input.x * slideAmount;
            controller.Move(rightDir * Time.deltaTime * currentHoverboardSpeed * 0.3f);
        }
    }

    public void Update()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space);

        CheckGroundDistance();
        if (distanceFromGround >= groundRaycastDistance)
        {
            playerTransform.rotation = Quaternion.Euler(0f, playerTransform.eulerAngles.y, 0f);
            hoverboardVelocity = Vector3.zero;
            currentHoverboardSpeed = 0f;
            return;
        }

        ApplyHoverPhysics(input);

        float currentMaxSpeed = isSprinting ? hoverboardSpeed * sprintMultiplier : hoverboardSpeed;

        if (Mathf.Abs(currentHoverboardSpeed) <= currentMaxSpeed)
        {
            if (input.y > 0.1f)
                currentHoverboardSpeed += hoverboardAcceleration * Time.deltaTime;
            else if (input.y < -0.1f)
                currentHoverboardSpeed -= hoverboardAcceleration * Time.deltaTime;
            else
                currentHoverboardSpeed *= (1.0f - hoverboardDeceleration * Time.deltaTime);
        }

        currentHoverboardSpeed = Mathf.Clamp(currentHoverboardSpeed, -currentMaxSpeed, currentMaxSpeed);
        Vector3 forwardMovement = playerTransform.forward * currentHoverboardSpeed;

        float turn = input.x * hoverboardTurnSpeed;
        playerTransform.Rotate(0, turn, 0);

        if (distanceFromGround > hoverHeight)
        {
            hoverboardVelocity.y += gravityValue * Time.deltaTime;
        }
        else if (distanceFromGround < hoverHeight)
        {
            float correctionForce = (hoverHeight - distanceFromGround) * 10.0f;
            hoverboardVelocity.y = Mathf.Lerp(hoverboardVelocity.y, correctionForce, Time.deltaTime * 10.0f);
        }
        else
        {
            hoverboardVelocity.y *= 0.9f;
        }

        bool isGrounded = distanceFromGround < hoverHeight * 1.5f;
        if (jumpPressed && isGrounded)
        {
            hoverboardVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        Vector3 movement = forwardMovement + new Vector3(0, hoverboardVelocity.y, 0);
        controller.Move(movement * Time.deltaTime);

        if (hoverParticles != null)
        {
            var emission = hoverParticles.emission;
            emission.rateOverTime = Mathf.Abs(currentHoverboardSpeed) * 5;
        }

        Quaternion targetRotation = Quaternion.FromToRotation(playerTransform.up, groundNormal) * playerTransform.rotation;
        playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, targetRotation, Time.deltaTime * 5.0f);
    }
}
