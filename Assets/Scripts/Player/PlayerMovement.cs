using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime; //How much tme the player can hang in the air before jumping
    private float coyoteCounter; //How much time passed since the playerran off the edge
    
    [Header("Multiple Jumps")]
    [SerializeField] private int extraJumps;
    private int jumpCounter;

    [Header("Wall Jumping")]
    [SerializeField] private float wallJumpX; //Horizontall wall jump force
    [SerializeField] private float wallJumpY; //Vertical wall jump force

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    
    [Header("Sound")]
    [SerializeField] private AudioClip jumpSound;
    
    private Rigidbody2D body;
    private Animator animator;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    private float horizontalInput;


    private void Awake()
    {
        //Grab references from the rigidbody and animator from the game object
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        //Flip the player when moving left or right
        if (horizontalInput > 0.01f) 
        {
            transform.localScale = Vector3.one;
        }else if (horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        //Set animation parameters 
        animator.SetBool("run", horizontalInput != 0);
        animator.SetBool("grounded", IsGrounded());

        //Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        //Adjustable jump height
        if(Input.GetKeyUp(KeyCode.Space) && body.velocity.y >0)
        {
            body.velocity = new Vector2(body.velocity.x, body.velocity.y / 2);
        }

        if (OnWall())
        {
            body.gravityScale = 0;
            body.velocity = Vector2.zero;
        }
        else
        {
            body.gravityScale = 7;
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
            
            if(IsGrounded())
            {
                coyoteCounter = coyoteTime; //Reset coyote when on the ground
                jumpCounter = extraJumps; // Reset the jump counter to the extra jump value
            }
            else
            {
                coyoteCounter -= Time.deltaTime; //Start decreasing coyote counter when not on the ground
            }
        }
    }

    private void Jump()
    {
        if (coyoteCounter < 0 && !OnWall() && jumpCounter <= 0)
        {
            return; //if coyote is 0 or less and not on the wall don't do anything
        }


        SoundManager.instance.PlaySound(jumpSound);
        if (OnWall())
        {
            WallJump();
        }
        else
        {
            if(IsGrounded())
            {
                body.velocity = new Vector2(body.velocity.x, jumpPower);
            }
            else
            {
                //If not on the ground and coyote counter is bigger than 0 do a normal jump
                if (coyoteCounter > 0)
                {
                    body.velocity = new Vector2(body.velocity.x, jumpPower);
                }
                else
                {
                    if (jumpCounter > 0) //If we have extra jumps then jump and decrease the jump counter
                    {
                        body.velocity = new Vector2(body.velocity.x, jumpPower);
                        jumpCounter--;
                    }
                }
            }
            //Reset coyote counter to 0 to avoid double jumps
            coyoteCounter = 0;
        }
    }

    private void WallJump()
    {
        body.AddForce(new Vector2(-Mathf.Sign(transform.localScale.x) * wallJumpX, wallJumpY));
        wallJumpCooldown = 0;
    }
    
    private bool IsGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool OnWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    public bool canAttack()
    {
        bool hasHorizontalInput = horizontalInput == 0;
        bool isGrounded = IsGrounded();
        bool onWall = OnWall();
        return hasHorizontalInput && isGrounded && !onWall;
    }


}
