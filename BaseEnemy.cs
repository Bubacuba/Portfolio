using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    public GameObject enemyPrefab;

    public float enemyHealth;
    public float enemyMaxHealth;
    public bool enemyCanMove;
    public bool enemyCanFire;
    public float enemyMovementSpeed;
    public bool enemyCanDropLoot;
}
