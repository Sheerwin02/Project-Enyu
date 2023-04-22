using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //[SerializeField] private float speed;
    //[SerializeField] private LayerMask groundLayer;
    private Rigidbody2D body;
    private Animator animator;
    private BoxCollider2D boxCollider;
    //private SpriteRenderer sprite;
    //MovementState currentState;

    [SerializeField] private LayerMask jumpableGround;

    // Running and jumping parameter
    private float directionX = 0f;
    public float jumpforce = 10f;
    [SerializeField] private float movespeed = 10.0f;
    public float maxJumpVelocity = 10.0f;

    // Dashing parameter
    public float dashSpeed = 20f;
    public float dashCooldown = 4f;
    public float dashDuration = .7f;
    public KeyCode dashKey = KeyCode.LeftShift;

    // Wall Running parameter
    //public float wallJumpForce = 10.0f;
    //public float wallSlideSpeed = 1.0f;
    //public float wallStickTime = 0.25f;
    //public float wallJumpTime = 0.5f;
    //public Transform groundCheck;
    //public Transform wallCheck;
    //public float groundCheckRadius = 0.1f;
    //public float wallCheckDistance = 0.1f;
    //private float wallStickTimer = 0.0f;
    //private float wallJumpTimer = 0.0f;
    //public LayerMask whatIsGround;

    private enum MovementState { idle, running, jumping, falling, dash, die }

    private void Awake()
    {
        // Get references from original body and object
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        //sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // Return in case in progress to respawn
        if (animator.GetInteger("state") == 5) return;

        directionX = Input.GetAxis("Horizontal");
        body.velocity = new Vector2(directionX *movespeed / 2,  body.velocity.y);

        // Prevent stuck by applying little cheat on character location
        preventStuck(body.velocity);
        print(body.velocity);

        //Get the current action of the character
        //currentState = (MovementState)animator.GetInteger("state");

        if (movespeed < 70)
        {
            movespeed += 1;
        }

        //Jump
        bool doubleJumpCheck = animator.GetBool("isDoubleJump");

        if (Input.GetButtonDown("Jump") && (IsGrounded() || !doubleJumpCheck))
        {
            body.velocity = new Vector2(body.velocity.x, jumpforce);
            preventStuck(body.velocity);

            // Enable doublejump again when touch ground
            if (IsGrounded())
            {
                //animator.SetBool("isJumping", false);
                animator.SetBool("isDoubleJump", false);
            }
            //else if (animator.GetBool("canWallJump"))
            //{
            //    //WallJumping();
            //}
            else
            {
                animator.SetBool("isDoubleJump", true);
            }

        }

        UpdateAnimationState();
    }


    private void UpdateAnimationState()
    {
        MovementState state = MovementState.idle;

        // Running
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
        {
            if (directionX > 0f)
            {
                state = MovementState.running;
                transform.localScale = Vector3.one;
            }
            else if (directionX < 0f)
            {
                state = MovementState.running;
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
        else if (directionX == 0)
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

        animator.SetBool("isDashing", false);

        animator.SetBool("isDashingCooldown", true);
        yield return new WaitForSeconds(dashCooldown);

        animator.SetBool("isDashingCooldown", false);
    }


    // Wall jumping
    //private void WallJumping()
    //{
    //    bool isTouchingWall = animator.GetBool("isTouchingWall");
    //    animator.SetBool("isTouchingWall", Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround));

    //    if (isTouchingWall && !IsGrounded())
    //    {
    //        animator.SetBool("isWallSliding", true);
    //    }
    //    else
    //    {
    //        animator.SetBool("isWallSliding", false);
    //    }

    //    if (animator.GetBool("isWallSliding"))
    //    {
    //        body.velocity = new Vector2(body.velocity.x, Mathf.Clamp(body.velocity.y, -wallSlideSpeed, float.MaxValue));
    //        wallStickTimer += Time.fixedDeltaTime;
    //        if (Input.GetKeyDown(KeyCode.Space))
    //        {
    //            animator.SetBool("canWallJump", true);
    //        }
    //    }
    //    else
    //    {
    //        wallStickTimer = 0.0f;
    //    }

    //    if (animator.GetBool("canWallJump") && wallStickTimer < wallStickTime)
    //    {
    //        body.velocity = new Vector2(-transform.right.x * wallJumpForce, jumpForce);
    //        isJumping = true;
    //        canWallJump = false;
    //        wallJumpTimer = 0.0f;
    //    }
    //    if (animator.GetBool("isJumping"))
    //    {
    //        wallJumpTimer += Time.fixedDeltaTime;
    //        if (wallJumpTimer > wallJumpTime)
    //        {
    //            isJumping = false;
    //        }
    //    }


    //}

    // Hardcoded function to prevent charcter from stucking in a place
    private void preventStuck(Vector2 velocity)
    {
        if (velocity.x != 0)
        {
            body.transform.position = new Vector2(body.transform.position.x + 0.01f, body.transform.position.y);
        }
        else if (velocity.y != 0)
        {
            body.transform.position = new Vector2(body.transform.position.x, body.transform.position.y + 0.01f);
        }
    }
}
