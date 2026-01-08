using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movespeed;
    public float dashSpeed;
    public float dashDuration;
    public float dashCooldown;

    public Animator animator;
    public SpriteRenderer sprite;

    public int _latestAnimStage;

    public bool isDashing;

    public GameObject playerWeapon;

    public AudioSource rollSoundEffect;
    public AudioSource walkSoundEffect;



    Rigidbody2D rb;
    Vector2 moveDirection;

    private void Awake()
    {
        isDashing = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isDashing)
        {
            return;
        }

        InputManager();

        if(Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Dash());
            rollSoundEffect.Play();
            
        }
    }

	private void FixedUpdate()
	{
		if (isDashing)
		{
            if(_latestAnimStage == 1) animator.SetInteger("movementDirection", 4);
            else if(_latestAnimStage == 3) animator.SetInteger("movementDirection", 5);
            else if(_latestAnimStage == 2) animator.SetInteger("movementDirection", 6);
            return;
		}
        else MovmentManager();
	}

	public void InputManager()
	{
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector2 (moveX, moveY).normalized;
	}

    public void MovmentManager()
    {
        if (moveDirection.y > 0)
        {
            animator.SetInteger("movementDirection", 3);
            _latestAnimStage = 3;
        }
        else if (moveDirection.y < 0)
        {
            animator.SetInteger("movementDirection", 1);

            _latestAnimStage = 1;
        }
        else if (moveDirection.x < 0)
        {
            animator.SetInteger("movementDirection", 2);
            _latestAnimStage = 2;
            sprite.flipX = true;
        }
        else if (moveDirection.x > 0)
        {
            animator.SetInteger("movementDirection", 2);
            _latestAnimStage = 2;
            sprite.flipX = false;
        }
        else
        {

            animator.SetInteger("movementDirection", 0);
        }

        if (moveDirection.x != 0 && !walkSoundEffect.isPlaying)
        {
            walkSoundEffect.Play();
        }
        else if (moveDirection.y != 0 && !walkSoundEffect.isPlaying)
        {
            walkSoundEffect.Play();
        }
        rb.velocity = new Vector2(moveDirection.x * movespeed, moveDirection.y * movespeed);
    }

    private IEnumerator Dash()
    {
        Debug.Log("Dash");
        isDashing = true;
        playerWeapon.SetActive(false);
		rb.velocity = new Vector2(moveDirection.x * dashSpeed, moveDirection.y * dashSpeed);
        yield return new WaitForSeconds(dashDuration);
        animator.SetInteger("movementDirection", _latestAnimStage);
        playerWeapon.SetActive(true);
        isDashing = false;
	}




}
