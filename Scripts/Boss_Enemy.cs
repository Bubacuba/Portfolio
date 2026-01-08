using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Boss_Enemy : MonoBehaviour
{
    [SerializeField] private float health = 100f;
    private float maxHealth;

    private Transform player;
    private Boss_Health_Bar healthBar;

    public static event Action OnBossDefeated;

    private void Start()
    {
        maxHealth = health;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("Player not found! Ensure player has 'Player' tag.");
        }

        GameObject hb = GameObject.Find("BossHealthGreen");
        if (hb != null)
        {
            healthBar = hb.GetComponent<Boss_Health_Bar>();
            healthBar.SetMaxHealth(maxHealth);
        }
        else
        {
            Debug.LogError("Health bar GameObject not found!");
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Health: {health}");

        if (healthBar != null)
            healthBar.SetHealth(health);

        if (health <= 0)
        {
            Destroy(gameObject);
            SceneManager.LoadScene(4);
        }
    }
}
//using System;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;

//public class Boss_Enemy : MonoBehaviour
//{
//    [SerializeField] private float health = 100f;
//    private float maxHealth;

//    private Transform player;
//    private Boss_Health_Bar healthBar;

//    // New: Event to notify when boss is defeated
//    public static event Action OnBossDefeated;

//    private void Start()
//    {
//        maxHealth = health;

//        player = GameObject.FindGameObjectWithTag("Player")?.transform;
//        if (player == null)
//        {
//            Debug.LogError("Player not found! Ensure player has 'Player' tag.");
//        }

//        GameObject hb = GameObject.Find("BossHealthGreen");
//        if (hb != null)
//        {
//            healthBar = hb.GetComponent<Boss_Health_Bar>();
//            healthBar.SetMaxHealth(maxHealth);
//        }
//        else
//        {
//            Debug.LogError("Health bar GameObject not found!");
//        }
//    }

//    public void TakeDamage(float damage)
//    {
//        health -= damage;
//        Debug.Log($"{gameObject.name} took {damage} damage. Health: {health}");

//        if (healthBar != null)
//            healthBar.SetHealth(health);

//        if (health <= 0)
//        {
//            Destroy(gameObject);

//            SceneManager.LoadScene(4);
//        }
//    }
//}
