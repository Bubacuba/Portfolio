using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Camera_Script : MonoBehaviour
{
    [Header("References")]
    public Transform playerObj;
    public Rigidbody rb;
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

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        defaultCameraPosition = cameraOffset;
        targetCameraPosition = cameraOffset;

        mainCamera.fieldOfView = normalFOV;
    }

    private void Update()
    {
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

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = cam.forward * verticalInput + cam.right * horizontalInput;
        moveDirection.y = 0;

        float currentSpeed = (Input.GetKey(KeyCode.LeftShift)) ? movementSpeed * sprintMultiplier : movementSpeed;
        rb.velocity = new Vector3(moveDirection.x * currentSpeed, rb.velocity.y, moveDirection.z * currentSpeed);
    }
}
