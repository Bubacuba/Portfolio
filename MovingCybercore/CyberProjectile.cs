using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyberProjectile : MonoBehaviour
{
    public float damage;
    public SpriteRenderer sprite;
    private float red;
    private float blue;
    private float green;

    public Animator anim;


    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        red = sprite.color.r;
        blue = sprite.color.b;
        green = sprite.color.g;
        StartCoroutine(ChangeColor());
    }

    IEnumerator ChangeColor()
    {
        if(gameObject != null)
        {
            for(int i = 0; i < 100; i++)
            {
                if(gameObject != null)
                {
                    //Debug.Log(sprite.color.r);
                    sprite.color = new Color(red, green, blue);
                    red -= 0.01f;
                    green -= 0.005f;
                    yield return new WaitForSeconds(0.05f);
                }
            }
        }
        yield return null;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log("Player Hit");
            var playerHpScript = collision.gameObject.GetComponent<PlayerHealth>();
            playerHpScript.TakeDamage(10);
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
