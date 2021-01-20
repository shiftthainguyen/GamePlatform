using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private float movementInputDirection;
    private int amountOfJumpLeft;
    private int facingDirection = 1;

    public float movementSpeed = 10.0f;  
    public float jumpForce = 16.0f;   
    public float groundCheckRadius;
    public float wallCheckDistance;
    public float wallSildeSpeed;
    public float movementForceInAir;
    public float airDragMultiplier =0.95f;
    public float variableJumpHeightMultiplier =0.5f;
    public float wallHopForce;
    public float wallJumpForce;


    private bool isGrounded;
    private bool isFacingRight = true;
    private bool isWalking;
    private bool canJump;
    private bool isTouchingWall;
    private bool isWallSilding;

    public int amountOfJump = 1;

    public Vector2 wallHopDirection;
    public Vector2 wallJumpDirection;

    public Transform groubdCheck;
    public Transform wallCheck;

    public LayerMask whatIsGround;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        amountOfJumpLeft = amountOfJump;
        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        UpdateAnimation();
        CheckCanJump();
        CheckWallSilding();
    }
    private void FixedUpdate() 
    {
        ApplyInput();
        CheckSurroundings();
    }
    private void CheckWallSilding()
    {
        if(isTouchingWall && !isGrounded && Mathf.Abs(rb.velocity.y) > 0)
        {
            isWallSilding = true;           
        }
        else
        {
            isWallSilding = false;          
        }
    }
    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groubdCheck.position, groundCheckRadius, whatIsGround);
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance,whatIsGround);
    }
    private void CheckCanJump()
    {
        if((isGrounded && rb.velocity.y <= 0) || isWallSilding )
        {
            amountOfJumpLeft = amountOfJump;          
        }
        if(amountOfJumpLeft <= 0)
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
        if(isFacingRight && movementInputDirection < 0)
        {
            Flip();
        }
        else if(!isFacingRight && movementInputDirection > 0)
        {
            Flip();
        }
        if(rb.velocity.x != 0)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }
    private void UpdateAnimation()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetFloat("speedMovement", Mathf.Abs(movementInputDirection));
        anim.SetBool("isWallSliding", isWallSilding);

    }
    private void Flip()
    {
        if (!isWallSilding)
        {
            facingDirection *= -1;
            isFacingRight = !isFacingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }  
    }
    private void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        if (Input.GetButtonUp("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplier);
        }
    }
    private void Jump()
    {
        if (canJump && !isWallSilding)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            amountOfJumpLeft--;
        }else if(isWallSilding && movementInputDirection ==0 && canJump) //wall hop
        {
            isWallSilding = false;
            amountOfJumpLeft--;
            Vector2 forceToAdd = new Vector2(wallHopForce * wallHopDirection.x * -facingDirection, wallHopForce * wallHopDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
        }else if((isWallSilding || isTouchingWall) && movementInputDirection != 0 && canJump)
        {
            isWallSilding = false;
            amountOfJumpLeft--;
            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * movementInputDirection, wallJumpForce * wallJumpDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
        }
    }
    private void ApplyInput()
    {

        if (isGrounded)
        {
            rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
        }
        else if (!isGrounded && !isWallSilding && movementInputDirection != 0) 
        {
            Vector2 foreToAdd = new Vector2(movementForceInAir * movementInputDirection, 0);
            rb.AddForce(foreToAdd);
            if (Mathf.Abs(rb.velocity.x) > movementSpeed)
            {
                rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
            }
        }       
        else if(!isGrounded && !isWallSilding && movementInputDirection == 0)
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
        }
        if (isWallSilding)
        {
            if(rb.velocity.y < -wallSildeSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSildeSpeed);
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groubdCheck.position, groundCheckRadius);

        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));

    }
}
