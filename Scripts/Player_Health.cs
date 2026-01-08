using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Health : MonoBehaviour
{
    public int playerHealth;
    public int playerMaxHealth = 100;

    // Start is called before the first frame update
    void Start()
    {
        playerHealth = playerMaxHealth;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerTakeDamage(int amount)
    {
        playerHealth -= amount;

        if(playerHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
