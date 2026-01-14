using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerTracker2 : MonoBehaviour
{
    public event Action OnGroundTouch;
    public event Action OnGroundLeave;
    public event Action OnWallTouch;
    public event Action OnWallLeave;

    private Rigidbody2D rb;
    private Collider2D col;

    private PlayerMovement2 myMov;
    private PlayerDash2 myDash;

    public LayerMask ground;
    public bool grounded;

    public int facingDir;

    [Header("Controls")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey = KeyCode.LeftShift;

    public float xInput;
    public float yInput;

    [Header("Jump Stuffs")]
    bool isJumping;
    public float jumpTime;
    float jumpTimer;

    [Header("Dash Stuffs")]
    bool isDashing;
    public float dashTime;
    float dashTimer;

    public Vector2 myPos;
    private Vector2 mySize;

    float halfHeight;
    float halfWidth;

    public float xVel, yVel;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        myMov = GetComponent<PlayerMovement2>();
        myDash = GetComponent<PlayerDash2>();
    }
    
    void FixedUpdate()
    {

    }

    // Update is called once per frame
    void Update()
    {
        myPos = rb.position;
        mySize = col.bounds.size;

        halfHeight = mySize.y / 2f;
        halfWidth = mySize.x / 2f;

        TakeInput();

        GroundCheck();

        Timers();
    }

    void GroundCheck()
    {
        float skinWidth = 0.02f;

        float boxBottom = myPos.y - halfHeight;
        float boxLeft = myPos.x - halfWidth;

        Vector2 rayOrigin = new Vector2(boxLeft + skinWidth, boxBottom - skinWidth);
        float rayLength = mySize.x - (2f * skinWidth);

        bool wasGrounded = grounded;

        grounded = Physics2D.Raycast(rayOrigin, Vector2.right, rayLength, ground);

        if (wasGrounded && !grounded)
            OnGroundLeave?.Invoke();
        if (!wasGrounded && grounded)
            OnGroundTouch?.Invoke();
    }



    void TakeInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        if (xInput != 0)
        {
            facingDir = (int)xInput;
            myMov.Move();
        }

        bool canJump = grounded;
        bool canDash = true;

        if (Input.GetKeyDown(jumpKey) && canJump)
        {
            StartJump();

        }
        if ((Input.GetKeyUp(jumpKey) || (jumpTimer == 0)) && isJumping)
        {
            StopJump();
        }

        if (Input.GetKeyDown(dashKey))
        {
            myMov.StartDash();
        }
    }

    void StartJump()
    {
        isJumping = true;
        myMov.StartJump();
        jumpTimer = jumpTime;
    }
    void StopJump()
    {
        isJumping = false;
        myMov.StopJump();
        jumpTimer = 0;
    }

    void Timers()
    {
        if (jumpTimer > 0)
            jumpTimer -= Time.deltaTime;
        else
            jumpTimer = 0;
        
        if (dashTimer > 0)
            dashTimer -= Time.deltaTime;
        else
            dashTimer = 0;
    }
}
