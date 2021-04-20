using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float jumpPower = 5f;
    [SerializeField] private bool isActive;
    
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float horizontal;
    private bool isFacingRight;
    private bool isGrounded;
    private int animLayer = 0;
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        isActive = true;
        isFacingRight = true;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        
        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
        }
    }

    private void FixedUpdate()
    {
        if (isGrounded)
        {
            horizontal = Input.GetAxis("Horizontal") * moveSpeed;
        }

        if (isActive)
        {
            Move();
            Flip();

            if (rigidbody.velocity.y < 0)
            {
                Fall();
            }
        }
    }
    
    public void Move()
    {
        if (!IsPlaying(animator, "Attack1") || !IsPlaying(animator, "Attack2")
                                            || IsPlaying(animator, "Attack3"))
        {
            if (horizontal != 0 && isGrounded)
            {
                rigidbody.velocity = new Vector2(horizontal, rigidbody.velocity.y);
                animator.SetBool("Running", true);
            }
            else if (!isGrounded)
            {
                rigidbody.velocity = new Vector2(horizontal, rigidbody.velocity.y);
            }
            else
            {
                animator.SetBool("Running", false);
            }
        }
    }

    private void Jump()
    {
        if (isGrounded)
        {
            rigidbody.velocity = new Vector2(horizontal, jumpPower);
            animator.SetTrigger("Jumping");
        }
    }

    private void Fall()
    {
        animator.SetBool("Falling", true);
    }

    private void Flip()
    {
        if (horizontal < 0 && isFacingRight)
        {
            spriteRenderer.flipX = true;
            isFacingRight = !isFacingRight;
        }
        else if (horizontal > 0 && !isFacingRight)
        {
            spriteRenderer.flipX = false;
            isFacingRight = !isFacingRight;
        }
    }

    private void Attack()
    {
        if (IsPlaying(animator, "Attack1Transition"))
        {
            print("Second attack");
            animator.SetTrigger("Attack2");
        }
        else if(IsPlaying(animator, "Attack2Transition"))
        {
            print("Third attack");
            animator.SetTrigger("Attack3");
        }
        else
        {
            animator.SetTrigger("Attack1");
        }
    }
    
    bool IsPlaying(Animator anim, string stateName)
    {
        if (anim.GetCurrentAnimatorStateInfo(animLayer).IsName(stateName) &&
            anim.GetCurrentAnimatorStateInfo(animLayer).normalizedTime < 1.0f)
            return true;
        else
            return false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.tag == "Ground")
        {
            isGrounded = true;
            animator.SetBool("Falling", false);
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.transform.tag == "Ground")
        {
            isGrounded = false;
        }
    }
}
