using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
	public static event Action OnPlayerDamaged;

	public float health, maxHealth;
    private bool deadMoveDone;

    public SpriteRenderer spriteRend;
    private float red;

    public AudioSource deathSFX;

    public GameObject wand;

    public PlayerHealthBar pHealthBar;
	private void Start()
    {
        deadMoveDone = false;
		health = maxHealth;
        red = spriteRend.color.r;
    }

    private void FixedUpdate()
    {
        if(health < 1)
        {
			var anim = gameObject.GetComponent<Animator>();
			anim.SetBool("triggerDeath", true);
            OnDeathFixPosition();
            var playerMovementScript = gameObject.GetComponent<PlayerMovement>();
            playerMovementScript.enabled = !playerMovementScript.enabled;
            var rb = gameObject.GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;
            if(!deathSFX.isPlaying) deathSFX.Play();
            wand.SetActive(false);
            Invoke("Destroyer", 1.25f);
            //Invoke("RestartGame", 1.25f); //Change this  to respawn or change scene
        }
    }

    void RestartGame()
    {
        var currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.ToString());
    }
    public void TakeDamage(float amount)
    {
        var playerObjScript = gameObject.GetComponent<PlayerHealthBar>();
        playerObjScript.ChangeLife(amount);
        health -= amount;
		OnPlayerDamaged?.Invoke();
        StartCoroutine(TakeDamageAnimation());
    }

	private void Destroyer()
	{
		Destroy(gameObject);
	}

    private void OnDeathFixPosition()
    {
        if (!deadMoveDone)
        {
            var playerLength = gameObject.transform.lossyScale;
            var playerTransform = gameObject.GetComponent<Transform>();
            playerTransform.transform.position = new Vector2(playerTransform.transform.position.x, playerTransform.transform.position.y + (float)1);
            deadMoveDone = true;
        }
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
