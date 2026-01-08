using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Logic : MonoBehaviour
{
    public Transform player;
    public float rotationSpeed = 5f;
    public bool smoothRotation = true;

    private Animator animator;

    public float roarDistance = 1000f;
    private bool hasRoared = false;
    private float roarCooldown = 10f;
    private float lastRoarTime = -Mathf.Infinity;

    public Ground_Attack spikeAttack;
    public BossThrow boulderAttack;

    public Transform target;

    public float timeBetweenAttacks = 4f;

    private int currentAttackIndex = 0;

    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(AttackLoop());
    }

    void Update()
    {
        if (player == null) return;

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            if (smoothRotation)
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            else
                transform.rotation = targetRotation;
        }

        BossRoar();
    }

    public void BossRoar()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= roarDistance && Time.time >= lastRoarTime + roarCooldown && !hasRoared)
        {
            hasRoared = true;
            lastRoarTime = Time.time;
            animator.Play("Boss_ReactionAnimation");
        }
    }

    private IEnumerator AttackLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenAttacks);

            if (currentAttackIndex == 0)
            {
                animator.Play("idle_1");
                spikeAttack.StartRippleSpikes(target, 25, 32, 5f, 75f, 0.08f);
            }
            else
            {
                // Boulder attack
                //Debug.Log("Boss: Boulder Throw");
                //animator.Play("Projectiles");
            }

            currentAttackIndex = (currentAttackIndex + 1) % 2;
        }
    }
}
