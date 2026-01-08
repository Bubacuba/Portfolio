using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyCybercore : BaseEnemy
{
    public GameObject cyberPrefab;
    public GameObject healthBar;
    public Slider healthBarSlider;
    private Transform playerPosition;
    public Transform playerAlwaysPosition;

    public GameObject enemyCyberProjectile;
    public float enemyCyberDamage;
    public float enemyCyberProjectileSpeed;
    public float enemyCyberFireRate;

    public float remainingDistance;
    public float currentTimeUsed;

    public float red;

    public Animator animator;

    public SpriteRenderer spriteRend;

    public AudioSource shootSoundEffect;

    public AudioSource takeDmgSfx;

    public Transform dmgPop;

    private bool playerInRange;
    void Start()
    {

        spriteRend = GetComponent<SpriteRenderer>();
        red = spriteRend.color.r;
        enemyHealth = 250;
        enemyMaxHealth = 250;
        enemyCanMove = true;
        enemyCanDropLoot = true;
        enemyMovementSpeed = 1;
        enemyPrefab = cyberPrefab;
        enemyCyberFireRate = 3f;
        enemyCyberDamage = 1;
        enemyCyberProjectileSpeed = 3;
        animator.SetBool("Alive", true);
        playerInRange = false;
    }

    private void FixedUpdate()
    {
        if (playerPosition != null)
        {
            float step = enemyMovementSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, playerPosition.position, step);
            animator.SetFloat("movementSpeed", Mathf.Abs(step));
        }
        else
        {
            animator.SetFloat("movementSpeed", 0);
        }
    }

    IEnumerator FireAtPlayer()
    {
        enemyMovementSpeed = 0;
        animator.SetBool("shootingAnimation", true);

        yield return new WaitForSeconds(0.05f);
        shootSoundEffect.Play();
        yield return new WaitForSeconds(0.45f);
        animator.SetBool("shootingAnimation", false);
        GameObject shooting = Instantiate(enemyCyberProjectile, transform.position, Quaternion.identity);
        Vector2 enemyPos = transform.position;
        Vector2 targetPos = playerAlwaysPosition.position;
        Vector2 direction = (targetPos - enemyPos).normalized;
        shooting.GetComponent<Rigidbody2D>().velocity = direction * enemyCyberProjectileSpeed;
        shooting.GetComponent<CyberProjectile>().damage = enemyCyberDamage;
        enemyMovementSpeed = 1;
        for (int i = 0; i < 100; i++)
        {
            if (shooting != null)
            {
                
                direction = (playerAlwaysPosition.position - shooting.transform.position).normalized;
                if(shooting.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Dynamic)
                {
                    shooting.GetComponent<Rigidbody2D>().velocity = direction * enemyCyberProjectileSpeed;
                    yield return new WaitForSeconds(0.05f);
                }
            }
        }
        if(shooting != null && shooting.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Dynamic)
        {
             shooting.GetComponent<Rigidbody2D>().velocity = direction * enemyCyberProjectileSpeed * 2;
        }
        yield return new WaitForSeconds(enemyCyberFireRate);
        if(playerInRange) StartCoroutine(FireAtPlayer());

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            playerPosition = collision.transform;
            StartCoroutine(FireAtPlayer());
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            playerPosition = null;
            playerInRange = false;
        }
    }
    public void TakeDamage(float damage)
    {
        takeDmgSfx.Play();
        healthBar.SetActive(true);
        enemyHealth -= damage;
        CheckDeath();
        healthBarSlider.value = HealthPercentage();
        StartCoroutine(DamagePopUp(damage));
        StartCoroutine(TakeDamageAnimation());

    }

    private void CheckDeath()
    {
        if (enemyHealth <= 0)
        {
            animator.SetBool("Alive", false);
            Invoke("Destroyer", 1.12f);
        }
    }

    private void Destroyer()
    {
        Destroy(gameObject);
    }

    private float HealthPercentage()
    {
        return (enemyHealth / enemyMaxHealth);
    }

    IEnumerator DamagePopUp(float damage)
    {
        Vector3 randomSpawn = new Vector3(gameObject.transform.position.x + Random.Range(-0.1f, 0.1f), gameObject.transform.position.y + 0.5f + Random.Range(-0.2f, 0.1f), gameObject.transform.position.z);
        Transform damagePopTransform = Instantiate(dmgPop, randomSpawn, Quaternion.identity);
        DamagePop damagePopup = damagePopTransform.GetComponent<DamagePop>();
        damagePopup.Setup(damage);
        yield return null;
    }

    IEnumerator TakeDamageAnimation()
    {
        if (gameObject != null)
        {
            for (int i = 0; i < 10; i++)
            {
                spriteRend.color = new Color(red, 0, red);
                red -= 0.04f;
                yield return new WaitForSeconds(0.025f);

            }
            for (int i = 0; i < 10; i++)
            {
                spriteRend.color = new Color(red, 0, red);
                red += 0.04f;
                yield return new WaitForSeconds(0.025f);

            }
            spriteRend.color = new Color(1, 1, 1);

        }
    }
}
