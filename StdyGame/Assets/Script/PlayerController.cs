using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private float movementInputDirection;
    private float jumpTimer;
    private float turnTimer;

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
    public float jumpTimerSet = 0.15f;
    public float turnTimerSet = 0.1f;


    private bool isGrounded;
    private bool isFacingRight = true;
    private bool isWalking;
    private bool canNormalJump;
    private bool canWallJump;
    private bool isTouchingWall;
    private bool isWallSilding;
    private bool isAttemptingToJump;
    private bool checkJumpMultiplier;

    private bool canMove;
    private bool canFlip;

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
        CheckJump();
    }
    private void FixedUpdate() 
    {
        ApplyInput();
        CheckSurroundings();
    }
    private void CheckWallSilding()
    {
        if(isTouchingWall && movementInputDirection == facingDirection && rb.velocity.y < 0)
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
        if(isGrounded && rb.velocity.y <= 0.01f)
        {
            amountOfJumpLeft = amountOfJump;          
        }
        if (isTouchingWall)
        {
            canWallJump = true;
        }
        if(amountOfJumpLeft <= 0)
        {
            canNormalJump = false;
        }
        else
        {
            canNormalJump = true;
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
        if (!isWallSilding && canFlip)
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
            if (isGrounded || (amountOfJumpLeft >0 && isTouchingWall))
            {
                NormalJump();
            }
            else
            {
                jumpTimer = jumpTimerSet;
                isAttemptingToJump = true;
            }
        }
        if(Input.GetButtonDown("Horizontal") && isTouchingWall)
        {
            if(isGrounded && movementInputDirection != facingDirection)
            {
                canFlip = false;
                canMove = false;

                turnTimer = turnTimerSet;
            }
        }
        if (!canMove)
        {
            turnTimer -= Time.deltaTime;
            if(turnTimer <= 0)
            {
                canMove = true;
                canFlip = true;
            }
        }
        if (checkJumpMultiplier && !Input.GetButton("Jump"))
        {
            checkJumpMultiplier = false;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplier);
        }
    }
    private void CheckJump()
    {
       if(jumpTimer > 0)
        {
            //wall jump
            if (!isGrounded && isTouchingWall && movementInputDirection != 0 && movementInputDirection != facingDirection)
            {
                WallJump();
            }else if (isGrounded)
            {
                NormalJump();
            }
        }
       if(isAttemptingToJump)
        {
            jumpTimer -= Time.deltaTime;
        }
    }
    private void NormalJump()
    {
        if (canNormalJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            amountOfJumpLeft--;
            jumpTimer = 0;
            isAttemptingToJump = false;
            checkJumpMultiplier = true;
        }
    }
    private void WallJump()
    {
        if (canWallJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0.0f);
            isWallSilding = false;
            amountOfJumpLeft = amountOfJump;
            amountOfJumpLeft--;
            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * movementInputDirection, wallJumpForce * wallJumpDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
            jumpTimer = 0;
            isAttemptingToJump = false;
            checkJumpMultiplier = true;
            turnTimer = 0;
            canMove = true;
            canFlip = true;
        }
    }
    private void ApplyInput()
    {

        if (!isGrounded && !isWallSilding && movementInputDirection == 0)
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
        }
        else if(canMove)
        {
            rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
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
