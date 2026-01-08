using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Tricks : MonoBehaviour
{
    Player_Movement_Script ground;
    public Animator animator; 
    public string[] animationNames;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void PlayRandomAnimation()
    {
        if (animationNames.Length == 0) return;

        int randomIndex = Random.Range(0, animationNames.Length);
        animator.Play(animationNames[randomIndex]);
    }

    // Update is called once per frame
    void Update()
    {
        if(!ground && Input.GetKey(KeyCode.Space))
        {
            PlayRandomAnimation();
        }
    }
}
