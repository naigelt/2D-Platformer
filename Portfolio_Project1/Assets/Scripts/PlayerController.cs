using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private float movementInputDirection;

    private int amountOfJumpsLeft;

    private bool isFacingRight = true;
    private bool isWalking;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool canJump;

    private Rigidbody2D rb;
    private Animator anim;

    public int amountOfJumps = 1;

    public float moveSpeed = 10f;
    public float jumpForce = 16f;
    public float groundCheckRadious;
    public float wallCheckDistance;
    public float wallSlideSpeed;

    public Transform groundCheck;
    public Transform wallCheck;

    public LayerMask whatIsGround;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        amountOfJumpsLeft = amountOfJumps;
    }

    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        UpdateAnimations();
        CheckIfCanJump();
        CheckIfWallSliding();

    }

    private void FixedUpdate()
    {
        ApplyMovement();
        CheckSurroundings();
        
    }

    private void CheckIfWallSliding()
    {
        if(isTouchingWall && !isGrounded && rb.velocity.y < 0)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadious, whatIsGround);

        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
    }

    private void CheckIfCanJump()
    {
        if(isGrounded && rb.velocity.y <= 0)
        {
            amountOfJumpsLeft = amountOfJumps;
        }
        if (amountOfJumpsLeft <= 0) 
        {
            canJump = false;
        }
        else
        {
            canJump = true;
        }
    }

    private void CheckMovementDirection()
    {
        if( isFacingRight && movementInputDirection < 0)
        {
            Flip();

        }
        else if (!isFacingRight && movementInputDirection > 0)
        {
            Flip();
        }

        if (rb.velocity.x != 0)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }

    private void UpdateAnimations()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isWallSliding", isWallSliding);
    }

    private void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }

    private void Jump()
    {
        if(canJump) 
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            amountOfJumpsLeft--;
        }
    }

    private void ApplyMovement()
    {
        rb.velocity = new Vector2(moveSpeed * movementInputDirection, rb.velocity.y);

        if (isWallSliding)
        {
            if(rb.velocity.y < -wallSlideSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            }
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadious);

        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance,  wallCheck.position.y, wallCheck.position.z));
    }
}
