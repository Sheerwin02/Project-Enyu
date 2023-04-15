using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private LayerMask groundLayer;
    private Rigidbody2D body;
    private Animator animator;
    private BoxCollider2D boxCollider;
    private SpriteRenderer sprite;
    private bool grounded;

    [SerializeField] private LayerMask jumpableGround;

    private float directionX = 0f;
    [SerializeField] public float jumpforce = 15;
    [SerializeField] private float movespeed = 10;

    private enum MovementState { idle, running, jumping, falling }

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

        print(movespeed);
        if (movespeed < 70)
        {
            movespeed += 1;
        }

        // Flip player when switching direction
        //if (horizontalInput > 0.001f)
        //{
        //    transform.localScale = Vector3.one;
        //}
        //else if (horizontalInput < -0.001f)
        //{
        //    transform.localScale = new Vector3(-1, 1, 1);
        //}

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            body.velocity = new Vector2(body.velocity.x, jumpforce);
        }
        //print(horizontalInput);

        UpdateAnimationState();

        // Set parameter for run
        //animator.SetBool("isRunning", horizontalInput != 0);
        //animator.SetBool("isGround", grounded);
    }

    //private void Jump()
    //{
    //    animator.SetTrigger("jump");
    //    body.velocity = new Vector2(body.velocity.x, speed);
    //    grounded = false;
    //}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            grounded = true;
        }
    }

    private void UpdateAnimationState()
    {
        MovementState state;

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

        if (body.velocity.y > .1f)
        {
            state = MovementState.jumping;
            movespeed = 30;

        }
        else if (body.velocity.y < -.1f)
        {
            state = MovementState.falling;
            movespeed = 30;
        }

        animator.SetInteger("state", (int)state);
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }
}
