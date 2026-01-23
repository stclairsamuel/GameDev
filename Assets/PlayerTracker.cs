using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerTracker : MonoBehaviour
{
    public event Action OnGroundTouch;
    public event Action OnGroundLeave;
    public event Action OnWallTouch;
    public event Action OnWallLeave;

    public event Action Attack;

    public event Action Jump;
    public event Action Dash;

    private Rigidbody2D rb;
    private Collider2D col;

    private PlayerMovement2 myMov;
    private PlayerDash2 myDash;
    private PlayerAttack myAttack;

    public LayerMask ground;
    public bool grounded;

    public int facingDir;

    public bool lockSpeed;

    [Header("Controls")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey = KeyCode.LeftShift;

    public float xInput;
    public float yInput;

    [Header("Jump Stuffs")]
    public bool isJumping;
    public float jumpTime;
    float jumpTimer;

    public float jumpBufferTime;
    float jumpBufferTimer;
    public float coyoteTime;
    public float coyoteTimer;

    public float dashJumpTime;
    float dashJumpTimer;
    public bool canDashJump;

    public float landingSpeedTime;
    public float landingSpeedTimer;

    [Header("WallStuffs")]
    public bool touchingWall;
    public int wallTouched;
    public int lastWallTouched;
    public bool canWallJump;

    public bool canDashWallJump;

    public float superWallJumpTime;
    public float superWallJumpTimer;

    [Header("Dash Stuffs")]
    public bool isDashing;
    public float dashTime;
    float dashTimer;

    public Vector2 myPos;
    private Vector2 mySize;

    public float halfHeight;
    public float halfWidth;

    public float savedXVel;

    void OnEnable()
    {
        myAttack.successfulHit += HitEnemy;
    }
    void OnDisable()
    {
        myAttack.successfulHit -= HitEnemy;
    }

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        myMov = GetComponent<PlayerMovement2>();
        myDash = GetComponent<PlayerDash2>();
        myAttack = GetComponentInChildren<PlayerAttack>();

        facingDir = 1;
    }
    
    void FixedUpdate()
    {
        if (xInput != 0)
        {
            facingDir = (int)xInput;
            myMov.Move();
        }
    }

    // Update is called once per frame
    void Update()
    {
        myPos = rb.position;
        mySize = col.bounds.size;

        halfHeight = mySize.y / 2f;
        halfWidth = mySize.x / 2f;



        TakeInput();

        lockSpeed = false;

        GroundCheck();
        WallsCheck();
        TopCheck();

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
        {
            GroundLeave();
            OnGroundLeave?.Invoke();
        }
        if (!wasGrounded && grounded)
        {
            GroundTouch();
            OnGroundTouch?.Invoke();
        }
    }

    void WallsCheck()
    {
        //Variables

        float skinWidth = 0.02f;

        float boxBottom = myPos.y - halfHeight;
        float boxLeft = myPos.x - halfWidth;
        float boxRight = myPos.x + halfWidth;

        Vector2 leftOrigin = new Vector2(boxLeft - skinWidth, boxBottom + skinWidth);
        Vector2 rightOrigin = new Vector2(boxRight + skinWidth, boxBottom + skinWidth);

        float rayLength = mySize.y - (2f * skinWidth);

        bool wasTouchingWall = touchingWall;

        bool touchingLeft = Physics2D.Raycast(leftOrigin, Vector2.up, rayLength, ground);
        bool touchingRight = Physics2D.Raycast(rightOrigin, Vector2.up, rayLength, ground);

        touchingWall = touchingLeft || touchingRight;

        //Set Wall Touched

        if (touchingWall)
        {
            wallTouched = (touchingLeft ? -1 : 1);
            canWallJump = true;
        }
        
        if (!touchingWall)
            wallTouched = 0;
        
        if (wallTouched != 0)
            lastWallTouched = wallTouched;
        
        if (!touchingWall && coyoteTimer == 0)
        {
            canWallJump = false;
        }



        if (touchingWall != wasTouchingWall)
        {
            if (touchingWall)
            {
                WallTouch();
                OnWallTouch?.Invoke();
            }
            else
            {
                WallLeave();
                OnWallLeave?.Invoke();
            }
        }
        if (touchingLeft)
            myMov.xVel = Mathf.Clamp(myMov.xVel, 0, Mathf.Infinity);
        if (touchingRight)
            myMov.xVel = Mathf.Clamp(myMov.xVel, -Mathf.Infinity, 0);
    }

    void TopCheck()
    {
        float skinWidth = 0.02f;

        float boxTop = myPos.y + halfHeight;
        float boxLeft = myPos.x - halfWidth;

        Vector2 rayOrigin = new Vector2(boxLeft + skinWidth, boxTop + skinWidth);
        float rayLength = mySize.x - (2f * skinWidth);

        bool topCheck = Physics2D.Raycast(rayOrigin, Vector2.right, rayLength, ground);

        if (topCheck)
        {
            myMov.yVel = Mathf.Clamp(myMov.yVel, -Mathf.Infinity, 0);
            if (isJumping)
                StopJump();
        }
    }

    void TakeInput()
    {
        canDashJump = dashJumpTimer > 0;

        xInput = Input.GetAxisRaw("Horizontal");

        isDashing = dashTimer > 0;

        bool canJump = grounded || touchingWall || coyoteTimer > 0;
        bool canDash = true;

        if (Input.GetKeyDown(jumpKey))
        {
            if (canJump)
                StartJump();
            else
                jumpBufferTimer = jumpBufferTime;
        }
        if (jumpBufferTimer > 0 && canJump)
        {
            StartJump();
        }

        if ((Input.GetKeyUp(jumpKey) || (jumpTimer == 0)) && isJumping)
        {
            StopJump();
        }
        if (Input.GetKeyUp(jumpKey))
        {
            jumpBufferTimer = 0;
        }

        if (Input.GetKeyDown(dashKey))
        {
            dashTimer = dashTime;
            dashJumpTimer = dashJumpTime;
            myMov.StartDash();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Attack?.Invoke();
        }

        if (xInput == -Mathf.Sign(myMov.xVel) && xInput != 0 && isDashing)
            dashTimer = 0;
    }

    void StartJump()
    {
        jumpBufferTimer = 0;
        isJumping = true;
        myMov.StartJump();
        Jump?.Invoke();
        jumpTimer = jumpTime;
    }
    void StopJump()
    {
        isJumping = false;
        myMov.StopJump();
        jumpTimer = 0;
    }

    void GroundTouch()
    {
        if (Mathf.Abs(myMov.xVel) > myMov.moveSpeed)
        {
            landingSpeedTimer = landingSpeedTime;
        }
        if (superWallJumpTimer > 0)
            superWallJumpTimer = 0;
    }
    void GroundLeave()
    {
        if (!isJumping)
            coyoteTimer = coyoteTime;
    }
    void WallTouch()
    {
        if (Mathf.Abs(myMov.xVel) > myMov.moveSpeed)
        {
            savedXVel = myMov.xVel;
            superWallJumpTimer = superWallJumpTime;
        }
        else
            superWallJumpTimer = 0;
    }
    void WallLeave()
    {
        if (isJumping)
        {
            if (superWallJumpTimer > 0)
            {
                float arbitraryLeeway = 0.1f;
                superWallJumpTimer = arbitraryLeeway;
            }
        }

        else
            coyoteTimer = coyoteTime;
    }

    public void Damage(DamageInfo info)
    {

    }

    public void HitEnemy()
    {
        myMov.xVel = Mathf.Clamp(myMov.xVel - facingDir * 8f, -facingDir * 8f, facingDir * 8f);
        //landingSpeedTimer = landingSpeedTime;
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
        
        if (dashJumpTimer > 0)
            dashJumpTimer -= Time.deltaTime;
        else
            dashJumpTimer = 0;
        
        if (jumpBufferTimer > 0)
            jumpBufferTimer -= Time.deltaTime;
        else
            jumpBufferTimer = 0;

        if (coyoteTimer > 0)
            coyoteTimer -= Time.deltaTime;
        else
            coyoteTimer = 0;

        if (landingSpeedTimer > 0)
            landingSpeedTimer -= Time.deltaTime;
        else
            landingSpeedTimer = 0;

        if (superWallJumpTimer > 0)
            superWallJumpTimer -= Time.deltaTime;
        else
            superWallJumpTimer = 0;
    }
}
