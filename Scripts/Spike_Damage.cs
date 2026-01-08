using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeDamage : MonoBehaviour
{
    public int damage = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player_Health health = other.GetComponent<Player_Health>();
            if (health != null)
            {
                health.PlayerTakeDamage(damage);
                Debug.Log("Player took spike damage!");
            }
        }
    }
}
