using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalProjectile : MonoBehaviour
{
    public float damage;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log("Player Hit");
            var playerHpScript = collision.gameObject.GetComponent<PlayerHealth>();
            playerHpScript.TakeDamage(50);
            Destroy(gameObject);
        }
        else if (collision.tag == "Wall")
        {
            Debug.Log("Wall Hit");
            //anim.SetInteger("Hit", 1);
            var anime = gameObject.GetComponent<Animator>();
            var rb = gameObject.GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;
            anime.SetInteger("Hit", 1);
            Invoke("Destroyer", 0.5f);

        }
        else if (collision.tag == "Props")
        {
            Debug.Log("Prop Hit");
            Destroy(gameObject);
        }
    }

    private void Destroyer()
    {
        Destroy(gameObject);
    }
}
