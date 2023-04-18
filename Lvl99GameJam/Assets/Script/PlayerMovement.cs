using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private LayerMask groundLayer;
    private Rigidbody2D body;
    private Animator animator;
    private BoxCollider2D boxCollider;
    private SpriteRenderer sprite;
    MovementState currentState;

    [SerializeField] private LayerMask jumpableGround;

    // Running and jumping parameter
    private float directionX = 0f;
    [SerializeField] public float jumpforce = 20f;
    [SerializeField] private float movespeed = 10f;

    // Dashing parameter
    public float dashSpeed = 20f;
    public float dashCooldown = 5f;
    public float dashDuration = 1f;
    public KeyCode dashKey = KeyCode.LeftShift;

    private enum MovementState { idle, running, jumping, falling, dash }

    private void Awake()
    {
        // Get references from original body and object
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        directionX = Input.GetAxis("Horizontal");
        body.velocity = new Vector2(directionX *movespeed / 2, body.velocity.y);

        // Get the current action of the character
        currentState = (MovementState)animator.GetInteger("state");

        //print(movespeed);
        if (movespeed < 70)
        {
            movespeed += 1;
        }

        //Jump
        bool doubleJumpCheck = animator.GetBool("isDoubleJump");

        if (Input.GetButtonDown("Jump") && (IsGrounded() || !doubleJumpCheck))
        {
            body.velocity = new Vector2(body.velocity.x, jumpforce);

            // Enable doublejump again when touch ground
            if (IsGrounded())
            {
                animator.SetBool("isDoubleJump", false);
            } else
            {
                animator.SetBool("isDoubleJump", true);
            }
            
        }

        UpdateAnimationState();
    }

    //private void Jump()
    //{
    //    animator.SetTrigger("jump");
    //    body.velocity = new Vector2(body.velocity.x, speed);
    //    grounded = false;
    //}

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.tag == "Ground")
    //    {
    //        grounded = true;
    //    }
    //}

    private void UpdateAnimationState()
    {
        MovementState state;

        // Running
        if (directionX > 0f)
        {
            state = MovementState.running;
            transform.localScale = Vector3.one;
            //sprite.flipX = false;
        }
        else if (directionX < 0f)
        {
            state = MovementState.running;
            //sprite.flipX = true;
            transform.localScale = new Vector3(-1,1,1);
        }
        else
        {
            state = MovementState.idle;
            movespeed = 10;
        }

        // Jumping
        if (body.velocity.y > .1f)
        {
            state = MovementState.jumping;
            movespeed = 30;

        }
        // Falling
        else if (body.velocity.y < -.1f)
        {
            state = MovementState.falling;
            movespeed = 30;
        }

        // Dash
        if (Input.GetKey(dashKey) && !animator.GetBool("isDashingCooldown"))
        {
            state = MovementState.dash;
            StartCoroutine(Dash());
        }

        animator.SetInteger("state", (int)state);
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }


    // Dash
    IEnumerator Dash()
    {
        // X direction
        float tempX = (directionX == 0) ? 1 : directionX;

        animator.SetBool("isDashing", true);

        body.velocity = new Vector2(40 * tempX, 0);

        yield return new WaitForSeconds(dashDuration);

        body.velocity = Vector2.zero;
        animator.SetBool("isDashing", false);

        animator.SetBool("isDashingCooldown", true);
        yield return new WaitForSeconds(dashCooldown);

        animator.SetBool("isDashingCooldown", false);
    }
}
