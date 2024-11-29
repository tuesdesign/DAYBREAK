using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigiAnimController : MonoBehaviour
{
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();    
    }

    public bool isMoving {
        get { return animator.GetBool("isMoving"); }
        set { animator.SetBool("isMoving", value); }
    }

    public bool isShooting
    {
        get { return animator.GetBool("isShooting"); }
        set { animator.SetBool("isShooting", value); }
    }

    public Vector2 moveDirection
    {
        get { return new Vector2(animator.GetFloat("moveX"), animator.GetFloat("moveY")); }

        set
        {
            animator.SetFloat("moveX", value.normalized.x);
            animator.SetFloat("moveY", value.normalized.y);
        }
    }
}
