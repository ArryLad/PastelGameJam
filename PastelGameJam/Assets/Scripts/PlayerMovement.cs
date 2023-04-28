using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Header("Components")]
    public Rigidbody2D rb;
    private SpriteRenderer sr;
    

    [Header("Layer Masks/Groun Check")]
    [SerializeField] private LayerMask groundLayer;

    [Header("Movement Variables")]
    [SerializeField] private float maxAcceleration;
    [SerializeField] private float firstAcceleration = 5f;
    [SerializeField] private float secondAcceleration = 10f;
    [SerializeField] private float thirdAcceleration = 25f;
    [SerializeField] public float maxSpeed;
    [SerializeField] private float firstSpeed = 10f;
    [SerializeField] private float secondSpeed = 25f;
    [SerializeField] public float thirdSpeed = 40f;
    [SerializeField] private float groundlinearDrag = 7f;
    private float horizontalDirection;
    public float directionTimer = 0.5f;


    [Header("Jump Varibles")]
    [SerializeField] private float jumpForce = 18f; //jump force
    [SerializeField] private float bounceForce = 14f; //bounce force when stomping on enemys
    [SerializeField] private float airLinearDrag = 3f; //drag while airborne
    [SerializeField] private float fallMultiplier = 10f; //gravity scale changer
    [SerializeField] private float lowFallMultiplier = 6f; //gravity scale changer
    public bool isFalling;
    public float lastYPos = 0;
    private bool canJump => Input.GetButtonDown("Jump") && (onGround || onSlope); //can jump bool true if pressing the jump button and on the ground

    [Header("Ground Collision Variables")]
    public bool onGround;
    [SerializeField] Transform groundCheck;
    [SerializeField] float checkDistance;
    

    [Header("Slope Components")]
    [SerializeField] float checkSlopeDistance;
    private float slopeDownAngle;
    private Vector2 slopeNormalPerp;
    [SerializeField] Transform slopeCheck;
    public bool onSlope;
    private float slopeSideAngle;
    //Store the hit information from our raycast, to use to update player's position
    private RaycastHit2D Hit2D;

    [Header("Friction Components")]
    [SerializeField] private PhysicsMaterial2D noFriction;
    [SerializeField] private PhysicsMaterial2D fullFriction;

    //if velocity is moving the direction opposite to the inputed direction the player is changing direction
    private bool changingDirection => rb.velocity.x > 0f && horizontalDirection < 0f || (rb.velocity.x < 0f && horizontalDirection > 0f);

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        maxSpeed = firstSpeed;
        maxAcceleration = firstAcceleration;
    }

    // Update is called once per frame
    void Update()
    {
        horizontalDirection = GetInput().x; //get current input of player

        if(horizontalDirection > 0f)
            sr.flipX = false;
        else if(horizontalDirection < 0f)
            sr.flipX = true;

        if (horizontalDirection == 0)
        {
            InputTimer();
        }
        else
            directionTimer = 0.5f;


        if (canJump)
            Jump();
    }

    private void FixedUpdate()
    {
        CheckCollisions(); //check what the player is colliding with 
        //CheckSlope();

        if (onGround) // if on the ground
            ApplyGroundLinearDrag(); //apply ground drag
        else //while airborne
        {
            ApplyAirLinearDrag(); //apply air drag
            FallMultiplier(); //and change gravity scale
        }

        lastYPos = transform.position.y; //check if the player Y is lower than it was last update
        MoveCharacter(); //move player function

            
    }

    /*private void CheckSlope()
    {
        CheckSlopeHorizontal();
        CheckSlopeVertical();
    }*/

    /*private void CheckSlopeHorizontal()
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(slopeCheck.position, transform.right, checkSlopeDistance, groundLayer);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(slopeCheck.position, -transform.right, checkSlopeDistance, groundLayer);

        if(slopeHitFront)
        {
            onSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
        }
        else if(slopeHitBack)
        {
            onSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
        }
        else
        {
            onSlope = false;
            slopeSideAngle = 0.0f;
        }

        if(onSlope && horizontalDirection == 0.0f)
        {
            rb.sharedMaterial = fullFriction;
        }
        else
        {
            rb.sharedMaterial = noFriction;
        }
    }*/
    /*private void CheckSlopeVertical()
    {
        RaycastHit2D hit = Physics2D.Raycast(slopeCheck.position, -transform.up, checkSlopeDistance, groundLayer);

        if (hit)
        {
            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            Debug.DrawRay(hit.point, slopeNormalPerp, Color.red);
            Debug.DrawRay(hit.point, hit.normal, Color.yellow);

            if (slopeDownAngle != 0)
            {
                onSlope = true;
            }
            else
            {
                onSlope = false;
            }
        }
    }*/
    private Vector2 GetInput()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); //get input horizontal which unity assigns to A/D and left/right arrow keys
    }

    private void InputTimer()
    {
        directionTimer -= Time.deltaTime;
        //Debug.Log(directionTimer.ToString());
        if (directionTimer <= 0.0f && horizontalDirection == 0)
        {
            maxSpeed = firstSpeed;
            maxAcceleration = firstAcceleration;
        }
    }

    private void MoveCharacter()
    {
        rb.AddForce(new Vector2(horizontalDirection, 0f) * maxAcceleration);
        /*
        if (onGround && !onSlope)
        {
            //move player's horizontal direction by acceleration
            rb.AddForce(new Vector2(horizontalDirection, 0f) * maxAcceleration);
        }
        else if(onGround && onSlope)
        {
            rb.AddForce(new Vector2(-horizontalDirection * slopeNormalPerp.x * maxAcceleration, -horizontalDirection * slopeNormalPerp.y * maxAcceleration));
        }
        else if (!onGround)
        {
            rb.AddForce(new Vector2(horizontalDirection * maxAcceleration, 0f));
        }
        */
        if (Mathf.Abs(rb.velocity.x) > maxSpeed) //player can accelerate to maxspeed but not over it
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);

        //if (Mathf.Abs(rb.velocity.x) == maxSpeed)
         //   IncreaseSpeed();
    }

    private void ApplyGroundLinearDrag()
    {
        if (Mathf.Abs(horizontalDirection) < 0.4f || changingDirection) //if moving apply drag
        {
            rb.drag = groundlinearDrag;
        }
        else //dont apply drag or player will drift
        {
            rb.drag = 0f;
        }
    }

    private void ApplyAirLinearDrag()
    {
        rb.drag = 0.5f;  //set drag to air linear drag
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0f); //halt vertical movement
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); //apply jumpforce upward
    }

    public void stompBounce()
    {
        //apply bounce force when stomp on enemy
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
    }

    private void FallMultiplier()
    {
        if (rb.velocity.y < 0) //if falling change gravity scale to fall multiplier
            rb.gravityScale = fallMultiplier;
        else if (rb.velocity.y > 0 && Input.GetButton("Jump"))
            rb.gravityScale = lowFallMultiplier;
    }
    private void CheckCollisions()
    {
        //check collision with ground layer using raycast
        onGround = Physics2D.Raycast(groundCheck.position, -transform.up, checkDistance, groundLayer);
        Hit2D = Physics2D.Raycast(groundCheck.position, -transform.up, checkDistance, groundLayer);
       /* if (onGround)
        {
            //create temp vector2 to store playerFeet position
            Vector2 temp = groundCheck.position;
            //We get the y position of our raycast hit/ and set the y value of our temp vector2
            temp.y = Hit2D.point.y;
            //Debug.Log(temp.y);
            //we can now directly set our players position by setting it to our temp vector2 value that we adjusted.
            rb.position = new Vector2(rb.position.x, temp.y+1f);
        }*/

        //if not on the ground and y posiiton is less than what it last was then the player is falling
        if (!onGround && transform.position.y < lastYPos)
            isFalling = true;
        if (onGround && isFalling) //if player has just landed
        {
            isFalling = false; //player is no longer falling
        }
    }

    public void IncreaseSpeed()
    {
        if (maxSpeed == firstSpeed)
        {
            maxSpeed = secondSpeed;
            maxAcceleration = secondAcceleration;
        }
        else if (maxSpeed == secondSpeed)
        {
            maxSpeed = thirdSpeed;
            maxAcceleration = thirdAcceleration;
        }
    }
    
}
