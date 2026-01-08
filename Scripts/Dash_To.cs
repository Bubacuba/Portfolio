using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash_To : MonoBehaviour
{
    [SerializeField] private float health = 1;
    

    private Transform player; 
    private float lastAttackTime; 
    private bool isPlayerInRange = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform; // Find player by tag
        if (player == null)
        {
            Debug.LogError("Player not found! Ensure player has 'Player' tag.");
        }
    }

    private void Update()
    {

    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Health: {health}");
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
