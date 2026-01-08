using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage_Script : MonoBehaviour
{
    public GameObject playerObject;
    private Player_Health playerHealth;

    public GameObject bossObject;
    private Enemy_Health enemyHealth;

    public int damage;

    // Start is called before the first frame update
    void Start()
    {
        bossObject = GameObject.FindWithTag("Boss");
        playerObject = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Collision detected between {gameObject.name} and {collision.gameObject.name}");

        // If Player collides with Boss -> Boss takes damage, Player takes NO damage
        if (gameObject == playerObject && collision.gameObject == bossObject)
        {
            Debug.Log("Player collided with Boss - NO DAMAGE TO PLAYER.");

            if (enemyHealth == null)
            {
                enemyHealth = bossObject.GetComponent<Enemy_Health>();
            }

            enemyHealth.EnemyTakeDamage(damage);
            Debug.Log("Boss takes damage.");

            return;
        }

        // If Player collides with an Enemy (not Boss) -> Player takes damage
        if (gameObject == playerObject && collision.gameObject.CompareTag("Enemy") && collision.gameObject != bossObject)
        {
            Debug.Log("Player collided with an Enemy - TAKING DAMAGE.");

            if (playerHealth == null)
            {
                playerHealth = playerObject.GetComponent<Player_Health>();
            }

            playerHealth.PlayerTakeDamage(damage);
            Destroy(collision.gameObject);
        }
    }
}
