using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate_Script : MonoBehaviour
{
    [SerializeField] private Transform boardTransform;
    [SerializeField] private Transform playerModel;     
    [SerializeField] private float playerOffsetY = 2f;

    public Transform cam;
    public Transform orientation;
    float horizontalInput;
    float verticalInput;
    public Transform playerObj;
    public float playerHeight;
    public LayerMask whatIsGround;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        RotatePlayer();
        if (boardTransform != null && playerModel != null)
        {
            MeshCollider boardCollider = boardTransform.GetComponent<MeshCollider>();
            float boardHeight = (boardCollider != null) ? boardCollider.bounds.extents.y : 0.2f; // Default to 0.2 if no collider

            float adjustedOffset = boardHeight + playerOffsetY;
            playerModel.position = boardTransform.position + boardTransform.up * adjustedOffset;
            playerModel.rotation = boardTransform.rotation;
        }
    }

    public void RotatePlayer()
    {
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        Vector3 slopeForward = playerObj.forward;
        Vector3 slopeNormal = Vector3.up;
        bool onSlope = false;

        int groundMask = LayerMask.GetMask("Ground");
        int ignoreLayer = LayerMask.GetMask("PlayerAndSword");
        int finalMask = groundMask & ~ignoreLayer;

        RaycastHit slopeHit;
        Debug.DrawRay(transform.position, Vector3.down * (playerHeight * 0.5f + 0.3f), Color.red);

        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, 2f, finalMask))
        {
            float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
            Debug.Log($"Hit: {slopeHit.collider.gameObject.name}, Slope: {slopeAngle}");

            if (slopeAngle > 0.1f)
            {
                onSlope = true;
                slopeNormal = slopeHit.normal;
                slopeForward = Vector3.ProjectOnPlane(cam.forward, slopeNormal).normalized;
            }
        }

        // Always face the camera
        Vector3 cameraForward = new Vector3(cam.forward.x, 0f, cam.forward.z).normalized;

        if (onSlope)
        {
            // Rotate player to align with slope normal and face slopeForward
            Quaternion targetRotation = Quaternion.LookRotation(slopeForward, slopeNormal);
            playerObj.rotation = Quaternion.Slerp(playerObj.rotation, targetRotation, Time.deltaTime * 10f);
        }
        else
        {
            // Rotate to face camera direction while standing upright
            Quaternion flatLookRotation = Quaternion.LookRotation(cameraForward, Vector3.up);
            playerObj.rotation = Quaternion.Slerp(playerObj.rotation, flatLookRotation, Time.deltaTime * 10f);
        }
    }
}
