using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnemyCrystal : BaseEnemy
{
    public GameObject crystalPrefab;
    public GameObject healthBar;
    public Slider healthBarSlider;
    public Transform playerPosition;

    public GameObject enemyCrystalProjectile1;
    public GameObject enemyCrystalProjectile2;
    public float enemyCrystalDamage;
    public float enemyCrystalProjectileSpeed;
    public float enemyCrystalFireRate;

    public float remainingDistance;
    public float currentTimeUsed;
    public Animator anim;
    public SpriteRenderer spriteRend;
    public float red;

    public AudioSource takeDmgSFX;
    public Transform dmgPop;

    private bool playerInRange;
    void Start()
    {
        enemyHealth = 500;
        enemyMaxHealth = 500;
        enemyCanMove = false;
        enemyCanDropLoot = true;
        enemyMovementSpeed = 0;
        enemyPrefab = crystalPrefab;
        enemyCrystalFireRate = 1f;
        enemyCrystalDamage = 1;
        enemyCrystalProjectileSpeed = 6;
        red = spriteRend.color.r;
        playerInRange = false;

}


IEnumerator FireAttack1AtPlayer()
    {
        yield return new WaitForSeconds(enemyCrystalFireRate);
        if(playerPosition != null)
        {
            anim.SetInteger("shootAnim", 1);
            yield return new WaitForSeconds(1f);
            anim.SetInteger("shootAnim", 0);
            GameObject fired = Instantiate(enemyCrystalProjectile1, transform.position, Quaternion.identity);
            Vector2 enemyPos = transform.position;
            Vector2 targetPos = playerPosition.position;
            Vector2 direction = (targetPos - enemyPos).normalized;
            fired.GetComponent<Rigidbody2D>().velocity = direction * enemyCrystalProjectileSpeed;
            fired.GetComponent<CrystalProjectile>().damage = enemyCrystalDamage;
            yield return new WaitForSeconds(1f);
            if (fired != null)
            {
                enemyPos = fired.transform.position;
                yield return new WaitForSeconds(0.2f);
                GameObject fired2 = Instantiate(enemyCrystalProjectile1, enemyPos, Quaternion.identity);
                targetPos = playerPosition.position;
                direction = (targetPos - enemyPos).normalized;
                fired2.GetComponent<Rigidbody2D>().velocity = direction * enemyCrystalProjectileSpeed;
                yield return new WaitForSeconds(0.2f);
                if (fired2 != null)
                {
                    enemyPos = fired2.transform.position;
                    yield return new WaitForSeconds(0.2f);
                    GameObject fired3 = Instantiate(enemyCrystalProjectile1, enemyPos, Quaternion.identity);
                    targetPos = playerPosition.position;
                    direction = (targetPos - enemyPos).normalized;
                    fired3.GetComponent<Rigidbody2D>().velocity = direction * enemyCrystalProjectileSpeed;
                    yield return new WaitForSeconds(0.2f);
                    if (fired3 != null)
                    {
                        enemyPos = fired3.transform.position;
                        yield return new WaitForSeconds(0.2f);
                        GameObject fired4 = Instantiate(enemyCrystalProjectile1, enemyPos, Quaternion.identity);
                        targetPos = playerPosition.position;
                        direction = (targetPos - enemyPos).normalized;
                        fired4.GetComponent<Rigidbody2D>().velocity = direction * enemyCrystalProjectileSpeed;
                        yield return new WaitForSeconds(0.2f);
                        if (fired4 != null)
                        {
                            enemyPos = fired4.transform.position;
                            yield return new WaitForSeconds(0.2f);
                            GameObject fired5 = Instantiate(enemyCrystalProjectile1, enemyPos, Quaternion.identity);
                            targetPos = playerPosition.position;
                            direction = (targetPos - enemyPos).normalized;
                            fired5.GetComponent<Rigidbody2D>().velocity = direction * enemyCrystalProjectileSpeed;
                        }
                       
                    }
                    
                }
               
            }
            if(playerInRange) StartCoroutine(FireAttack2AtPlayer());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine(FireAttack1AtPlayer());
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerInRange = false;
        }
    }
    IEnumerator FireAttack2AtPlayer()
    {
        yield return new WaitForSeconds(enemyCrystalFireRate);
        if (playerPosition != null)
        {

            for (int i = 0; i < 7; i++)
            {
                anim.SetInteger("shootAnim", 2);
                yield return new WaitForSeconds(0.3f);
                anim.SetInteger("shootAnim", 0);
                GameObject fired1 = Instantiate(enemyCrystalProjectile2, transform.position, Quaternion.identity);
                Vector2 enemyPos = transform.position;
                Vector2 targetPos = playerPosition.position;
                Vector2 direction = (targetPos - enemyPos).normalized;
                fired1.GetComponent<Rigidbody2D>().velocity = direction * enemyCrystalProjectileSpeed;
                fired1.GetComponent<CrystalProjectile>().damage = enemyCrystalDamage;
            }
            if(playerInRange) StartCoroutine(FireAttack1AtPlayer());
        }
    }

    IEnumerator DamagePopUp(float damage)
    {
        Vector3 randomSpawn = new Vector3(gameObject.transform.position.x + Random.Range(-0.1f, 0.1f), gameObject.transform.position.y + 0.5f + Random.Range(-0.2f, 0.1f), gameObject.transform.position.z);
        Transform damagePopTransform = Instantiate(dmgPop, randomSpawn, Quaternion.identity);
        DamagePop damagePopup = damagePopTransform.GetComponent<DamagePop>();
        damagePopup.Setup(damage);
        yield return null;
    }
    public void TakeDamage(float damage)
    {
        takeDmgSFX.Play();
        healthBar.SetActive(true);
        enemyHealth -= damage;
        CheckDeath();
        healthBarSlider.value = HealthPercentage();
        StartCoroutine(DamagePopUp(damage));
        StartCoroutine(TakeDamageAnimation());

    }

    private void CheckDeath()
    {
        if(enemyHealth <= 0)
        {
            anim.SetInteger("shootAnim", 3);
            Invoke("Destroyer", 0.625f);
        }
    }

    void Destroyer()
    {
        Destroy(gameObject);
    }
    private float HealthPercentage()
    {
        return (enemyHealth / enemyMaxHealth);
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
