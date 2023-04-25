using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Header("Components")]
    public Rigidbody2D rb;

    [Header("Layer Masks/Groun Check")]
    [SerializeField] private LayerMask groundLayer;

    [Header("Movement Variables")]
    [SerializeField] private float maxAcceleration;
    [SerializeField] private float firstAcceleration = 5f;
    [SerializeField] private float secondAcceleration = 20f;
    [SerializeField] private float thirdAcceleration = 75f;
    [SerializeField] public float maxSpeed;
    [SerializeField] private float firstSpeed = 10f;
    [SerializeField] private float secondSpeed = 40f;
    [SerializeField] public float thirdSpeed = 148f;
    [SerializeField] private float groundlinearDrag = 5f;
    private float horizontalDirection;
    public float directionTimer = 0.5f;


    [Header("Jump Varibles")]
    [SerializeField] private float jumpForce = 12f; //jump force
    [SerializeField] private float bounceForce = 12f; //bounce force when stomping on enemys
    [SerializeField] private float airLinearDrag = 2.5f; //drag while airborne
    [SerializeField] private float fallMultiplier = 8f; //gravity scale changer
    [SerializeField] private float lowFallMultiplier = 5f; //gravity scale changer
    public bool isFalling;
    public float lastYPos = 0;
    private bool canJump => Input.GetButtonDown("Jump") && onGround; //can jump bool true if pressing the jump button and on the ground

    [Header("Ground Collision Variables")]
    public bool onGround;
    [SerializeField] Transform groundCheck;
    [SerializeField] float checkDistance;

    //if velocity is moving the direction opposite to the inputed direction the player is changing direction
    private bool changingDirection => rb.velocity.x > 0f && horizontalDirection < 0f || (rb.velocity.x < 0f && horizontalDirection > 0f);

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        maxSpeed = firstSpeed;
        maxAcceleration = firstAcceleration;
    }

    // Update is called once per frame
    void Update()
    {
        horizontalDirection = GetInput().x; //get current input of player

        if (horizontalDirection == 0)
            InputTimer();
        else
            directionTimer = 0.5f;


        if (canJump)
            Jump();
    }

    private void FixedUpdate()
    {
        CheckCollisions(); //check what the player is colliding with 
        lastYPos = transform.position.y; //check if the player Y is lower than it was last update
        MoveCharacter(); //move player function

        if (onGround) // if on the ground
            ApplyGroundLinearDrag(); //apply ground drag
        else //while airborne
        {
            ApplyAirLinearDrag(); //apply air drag
            FallMultiplier(); //and change gravity scale
        }
    }

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
        //move player's horizontal direction by acceleration
        rb.AddForce(new Vector2(horizontalDirection, 0f) * maxAcceleration);

        if (Mathf.Abs(rb.velocity.x) > maxSpeed) //player can accelerate to maxspeed but not over it
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);

        if (Mathf.Abs(rb.velocity.x) == maxSpeed)
            IncreaseSpeed();
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
        else if(rb.velocity.y > 0 && !Input.GetButton("Jump"))
            rb.gravityScale = lowFallMultiplier;
        else
            rb.gravityScale = 2.5f;
    }
    private void CheckCollisions()
    {
        //check collision with ground layer using raycast
        onGround = Physics2D.Raycast(groundCheck.position, -transform.up, checkDistance, groundLayer);

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
