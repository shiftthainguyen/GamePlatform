using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private float movementInputDirection;
    private int amountOfJumpLeft;

    public float movementSpeed = 10.0f;  
    public float jumpForce = 16.0f;   
    public float groundCheckRadius;


    private bool isGrounded;
    private bool isFacingRight = true;
    private bool isWalking;
    private bool canJump;

    public int amountOfJump = 1;

    public Transform groubdCheck;
    public LayerMask whatIsGround;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        amountOfJumpLeft = amountOfJump;
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        UpdateAnimation();
        CheckCanJump();
    }
    private void FixedUpdate()
    {
        ApplyInput();
        CheckSurroundings();
    }
    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groubdCheck.position, groundCheckRadius, whatIsGround);
    }
    private void CheckCanJump()
    {
        if( isGrounded&& rb.velocity.y <=0)
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
    }
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0, 180, 0);
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
        if (canJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            amountOfJumpLeft--;
        }
        
    }
    private void ApplyInput()
    {
        rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groubdCheck.position, groundCheckRadius);
    }
}
