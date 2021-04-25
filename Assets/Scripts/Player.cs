using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigidbody; //variabilele marcate prin serializefield sunt private, dar pot fi vazute in inspector
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float jumpPower = 5f;
    [SerializeField] private bool isActive;
    
    private Animator animator; //variabilele private nu pot fi vazute in inspector
    private SpriteRenderer spriteRenderer;
    private float horizontal;
    private bool isFacingRight;
    private bool isGrounded;
    private int animLayer = 0;
    private bool canDoubleJump;
    private float pushForce = 10;
    private Vector2 pushDirection;
    private void Awake()   //instructiunile din awake se realizeaza inainte ca jocul sa se porneasca
    {
        rigidbody = GetComponent<Rigidbody2D>(); //facem referinta variabilei la componenta care se afla pe obiectul scriptului
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        isActive = true;
        isFacingRight = true;
    }

    private void Update()   //instructiunile din update au loc in fiecare frame
    {
        if (Input.GetButtonDown("Jump"))  //atunci cand tastam butonul Space (in editorul unity "Jump")
        {
            Jump();
        }
        
        if (isGrounded && Input.GetButtonDown("Fire1"))  //daca playerul este pe pamant si este tastat butonul Ctrl sau Right Mouse
        {
            Attack();
        }
        
        if (!isGrounded && Input.GetButtonDown("Fire1"))
        {
            FallingAttack();
        }
    }

    private void FixedUpdate() //asemanator cu Update, dar nu este legat de frame-uri
    {
        if (isActive)
        {
            if (isGrounded)
            {
                horizontal = Input.GetAxis("Horizontal") * moveSpeed;  //horizontal primeste directia de miscare pe axa orizontala inmultita cu viteza pe care o setam la alegere
            }
            
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
            animator.SetBool("Jumping", true);
            canDoubleJump = true;
        }
        else if (canDoubleJump)
        {
            rigidbody.velocity = new Vector2(horizontal, jumpPower);
            animator.SetBool("Jumping", true);
            canDoubleJump = false;
        }
        
    }

    private void Fall()
    {
        animator.SetBool("Jumping", false);
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

    private void FallingAttack()
    {
        animator.SetTrigger("FallingAttack");
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

        if (other.transform.tag == "Spikes")
        {
            animator.SetTrigger("GetHit");

            if (isFacingRight)
            {
                pushDirection = Vector2.left;
            }
            else
            {
                pushDirection = Vector2.right;
            }
            pushDirection = pushDirection.normalized;
            
            rigidbody.velocity = pushDirection * pushForce;
        }
    }

    private void OnCollisionStay2D(Collision2D other)
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
