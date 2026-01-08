using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderCollision : MonoBehaviour
{
    private int amount = 999;
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Collision detected between {gameObject.name} and {collision.gameObject.name}");

        if (collision.gameObject.CompareTag("Player"))
        {
            Player_Health playerHealth = collision.gameObject.GetComponent<Player_Health>();
            if (playerHealth != null)
            {
                playerHealth.PlayerTakeDamage(amount);
            }
        }

        Destroy(gameObject);
    }
}
