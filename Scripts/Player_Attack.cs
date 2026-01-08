using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player_Attack : MonoBehaviour
{
    [Header("Homing Settings")]
    public float homingRange = 15f;
    public float homingSpeed = 25f;
    public float homingCooldown = 2f;

    private Transform targetEnemy;
    private Rigidbody rb;
    private bool isHoming;
    private float lastHomingTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            TryStartHoming();
        }
    }

    void FixedUpdate()
    {
        if (isHoming && targetEnemy != null)
        {
            Vector3 direction = (targetEnemy.position - transform.position).normalized;
            rb.velocity = direction * homingSpeed;

            if (Vector3.Distance(transform.position, targetEnemy.position) < 1.5f)
            {
                StopHoming();
            }
        }
    }

    void TryStartHoming()
    {
        if (Time.time < lastHomingTime + homingCooldown) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, homingRange);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy") || hit.CompareTag("Boss"))
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestEnemy = hit.transform;
                }
            }
        }

        if (closestEnemy != null)
        {
            targetEnemy = closestEnemy;
            isHoming = true;
            lastHomingTime = Time.time;
        }
    }

    public void StopHoming()
    {
        isHoming = false;
        targetEnemy = null;
    }

    public bool IsHoming() => isHoming;
}
