using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    public int enemyHealth;
    public int enemyMaxHealth = 10;
    // Start is called before the first frame update
    void Start()
    {
        enemyHealth = enemyMaxHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void EnemyTakeDamage(int amount)
    {
        enemyHealth -= amount;

        if (enemyHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
