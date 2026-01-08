using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossThrow : MonoBehaviour
{
    public GameObject boulderPrefab;
    public Transform rightHandThrowPoint;
    public Transform leftHandThrowPoint;
    public float throwForce = 100f;
    public float throwHeight = 5f;

    private Transform player;
    private bool useRightHand = true;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ThrowBoulderAtPlayer();
        }
    }

    public void ThrowBoulderAtPlayer()
    {
        Transform throwPoint = useRightHand ? rightHandThrowPoint : leftHandThrowPoint;

        GameObject boulder = Instantiate(boulderPrefab, throwPoint.position, Quaternion.identity);
        Rigidbody rb = boulder.GetComponent<Rigidbody>();
        if (rb == null) rb = boulder.AddComponent<Rigidbody>();

        Vector3 directionToPlayer = player.position - throwPoint.position;
        directionToPlayer.y += throwHeight;
        directionToPlayer.Normalize();

        rb.velocity = directionToPlayer * throwForce;
        rb.useGravity = true;

        useRightHand = !useRightHand;
    }
}