using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region
    public Rigidbody2D rb;
    public Animator anim;
    public Transform groundCheck;
    public LayerMask whatIsLayer;
    #endregion

    #region
    public float movementSpeed;
    public float jumpForce;
    public float groundCheckRadius;
   


    public bool isGrounded;
    public bool canJump;
    public bool facingRight = true;
    #endregion

    #region
    private float movementInputDirection;


    
    #endregion

   

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        UpdateAnimation();
        CheckSurroundings();
        Jump();
    }

    void FixedUpdate()
    {
        Movement();
    }
    private void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump"))
        {
            canJump = true;
        }

    }
    private void UpdateAnimation()
    {
        anim.SetFloat("movementSpeed", Mathf.Abs(movementInputDirection));
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isGrounded", isGrounded);
    }
    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsLayer);
    }
    private void Movement()
    {
        rb.velocity = new Vector2(movementInputDirection * movementSpeed, rb.velocity.y);
        if(facingRight && movementInputDirection <0)
        {
            Flip();
        }
        else if(!facingRight && movementInputDirection >0)
        {
            Flip();
        }
    }
    private void Jump()
    {
        if (canJump && isGrounded)
        {

            rb.velocity = new Vector2(rb.velocity.x,jumpForce);
            canJump = false;
        }
    }
    private void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
