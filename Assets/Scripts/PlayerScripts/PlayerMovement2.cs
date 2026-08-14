using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2 : MonoBehaviour
{
    private PlayerTracker myTracker;

    private Rigidbody2D rb;

    public float gravity;
    public float fastFallMult;
    public float terminalVel;

    public float jumpForce;
    public bool isJumping;

    public float normalAccel;
    public float accelMod;
    public float dashAccelMod;
    public float moveSpeed;
    public float rushSpeed;

    public float groundDrag;
    public float groundIdleDrag;
    public float airDrag;
    public float airIdleDrag;

    public bool useThrowDrag;
    public float activeThrowDrag;
    public float throwDragMod;

    public float xVel;
    public float yVel;

    private bool fastFall;

    public float dashForce;

    public float savedXVel;

    private bool grounded;
    private bool touchingWall;

    private float xInput;

    void OnEnable()
    {
        myTracker.OnGroundTouch += GroundTouch;
        myTracker.OnWallTouch += WallTouch;
    }
    void OnDisable()
    {
        myTracker.OnGroundTouch -= GroundTouch;
        myTracker.OnWallTouch -= WallTouch;
    }

    void Awake()
    {
        myTracker = GetComponent<PlayerTracker>();

        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        grounded = myTracker.grounded;
        touchingWall = myTracker.touchingWall;

        fastFall = yVel < 0;
    }

    void FixedUpdate()
    {
        Gravity();

        Drag();

        if (myTracker.superWallJumpTimer > 0 && xInput == -myTracker.lastWallTouched && myTracker.isJumping)
        {
            xVel = -myTracker.lastWallTouched * (Mathf.Abs(myTracker.savedXVel));
            myTracker.superWallJumpTimer = 0;
        }

        rb.velocity = new Vector2(xVel, yVel);
    }

    public void StartJump()
    {
        isJumping = true;
        yVel = jumpForce; 

        if (myTracker.canWallJump)
        {
            int wallDir = myTracker.lastWallTouched;

            bool superWallJump = myTracker.superWallJumpTimer > 0;

            if (xInput == -wallDir && superWallJump)
            {
                xVel = -wallDir * (Mathf.Abs(myTracker.savedXVel));
                myTracker.superWallJumpTimer = 0;
            }
            else
                xVel = -wallDir * moveSpeed;
        }
    }
    public void StopJump()
    {
        isJumping = false;
        yVel *= 0.3f;
    }

    public void StartDash()
    {
        xVel = dashForce * myTracker.facingDir;
        if (yVel > 0)
            yVel *= 0.5f;
        else
            yVel = 0;
        accelMod = -normalAccel;
    }

    public void Move()
    {
        float speedToLock = moveSpeed;

        if (myTracker.isRushing)
            speedToLock = rushSpeed;


        bool lockSpeed = myTracker.lockSpeed;

        float acceleration = normalAccel + accelMod;

        if (myTracker.isRushing)
            acceleration = 200f;

        if (!lockSpeed)
        {
            if (xVel > moveSpeed || xVel < -moveSpeed)
            {
                speedToLock = Mathf.Abs(xVel);
            }
        }

        xVel = Mathf.Clamp(xVel + acceleration * myTracker.facingDir * Time.fixedDeltaTime, -speedToLock, speedToLock);

        CheckPos();
    }

    public void CheckPos()
    {
        Vector2 myPos = myTracker.myPos;
        int facingDir = myTracker.facingDir;

        float halfWidth = myTracker.halfWidth;
        float halfHeight = myTracker.halfHeight;

        float skinWidth = 0.02f;
        Vector2 topRayOrigin = new Vector2(myPos.x + (facingDir * (halfWidth + skinWidth)), myPos.y + (halfHeight / 2f));

        float rayLength = (halfHeight / 2f);

        RaycastHit2D topCheckUp = Physics2D.Raycast(topRayOrigin, Vector2.up, rayLength, myTracker.ground);
        RaycastHit2D topCheckIn = Physics2D.Raycast(topRayOrigin, Vector2.left * facingDir, skinWidth, myTracker.ground);

        if (topCheckUp && !topCheckIn)
        {
            Vector2 correctedPos = myPos - new Vector2(0, (rayLength - topCheckUp.distance) + skinWidth);
            transform.position = correctedPos;
        }

    }

    public void Drag()
    {
        bool keepLandingSpeed = myTracker.landingSpeedTimer > 0;

        int xInput = (int)Input.GetAxisRaw("Horizontal");

        Vector2 dragToUse = myTracker.GetDrag();

        xVel *= Mathf.Exp(-dragToUse.x * Time.fixedDeltaTime);

        yVel *= Mathf.Exp(-dragToUse.y * Time.fixedDeltaTime);
    }

    void Gravity()
    {
        float gravToUse = gravity;

        bool useThrowGrav = myTracker.isThrowing || myTracker.holdingThrow;

        bool useDashAtkGrav = myTracker.dashAtkDragTimer > 0;

        if (useThrowGrav)
        {
            float throwDragPercent = myTracker.throwStallTimer / myTracker.throwStallTime;

            float gravPow = 4f;
            float gravMult = 2f;

            gravToUse = Mathf.Pow(1f - throwDragPercent, gravPow) * gravity * gravMult;
        }

        if (useDashAtkGrav)
        {
            float dashTimerPercent = myTracker.dashAtkDragTimer / myTracker.dashAtkDragTime;

            gravToUse = (1f - dashTimerPercent) * gravity;
        }
        
        if (myTracker.impactedWall && myTracker.touchingWall)
        {
            gravToUse = 0;
        }
        
        xInput = (int)Input.GetAxisRaw("Horizontal");

        if (!grounded)
        {
            if (touchingWall && xInput == myTracker.lastWallTouched && yVel <= -2f)
            {
                float wallDrag = -7f;
                yVel *= Mathf.Exp(wallDrag * Time.fixedDeltaTime);
                if (yVel > -2f)
                    yVel = -2f;
            }
            else
            {
                yVel = Mathf.Clamp(yVel - (fastFall ? gravToUse * Time.fixedDeltaTime * fastFallMult : gravToUse * Time.fixedDeltaTime), -terminalVel, Mathf.Infinity);
            }
        }
        else if ((grounded || touchingWall) && yVel < 0)
        {
            yVel = -2f;
        }

    }

    void GroundTouch()
    {
        fastFall = false;
    }

    void WallTouch()
    {

    }
    void WallLeave()
    {

    }
}
