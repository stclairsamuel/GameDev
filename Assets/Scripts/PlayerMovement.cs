using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;

    public float xVel;
    public float yVel;
    
    public float baseMoveSpeed;
    public float apexSpeed;
    public float activeMoveSpeed;
    public bool lockSpeed;
    public float airMovMult;

    public Vector3 myPos;

    public KeyCode jumpKey = KeyCode.Space;

    public bool canJump;
    public bool isJumping;
    public float jumpForce;
    public float dashWallJumpForce;
    public float jumpTime;
    public float jumpTimer;

    public float wallJumpXForce;
    public float normalWallJumpXForce;
    public float dashWallJumpXForce;

    public float releaseJumpMultiplier;

    public int jumpDir;

    public float xInput;
    public float externalXVel;

    public float facingDir;

    public bool takeInput;

    public PlayerTracker pTracker;

    public Vector2 myScale;

    public GameObject colliderLeft;
    public GameObject colliderRight;

    public float lockSpeedTime;
    public float lockSpeedTimer;

    public float coyoteTime;
    public float coyoteTimer;
    public int coyoteTimeWall;
    public float lastWallJump;
    public float apexBoostTime;
    public float apexBoostTimer;
    public float wallJumpBoostTime;
    public float wallJumpBoostTimer;

    // Start is called before the first frame update
    void Start()
    {
        activeMoveSpeed = baseMoveSpeed;
    }

    // Update is called once per frame
    void Update()
    {

        myPos = transform.position;

        SpeedCheck();

        JumpChecks();

        if (takeInput)
        {
            GetInput();
        }
        else
        {
            xInput = 0;
        }

        if (Input.GetKeyUp(jumpKey))
        {
            pTracker.jumpBufferTimer = 0;

            if (isJumping)
            {
                JumpStop();
            }

            if (jumpDir == 1)
            {
                yVel *= releaseJumpMultiplier;
            }
        }

        if (xInput != 0)
        {
            facingDir = Mathf.Sign(xVel);
        }

        yVel = pTracker.ReturnYVel(yVel);

        rb.velocity = new Vector2(pTracker.ReturnXVel(xVel), yVel);

        Timers();
    }

    void SpeedCheck()
    {
        wallJumpXForce = pTracker.dashWallJumptimer > 0 ? dashWallJumpXForce : normalWallJumpXForce;

        if (pTracker.storedVelTimer > 0 && Mathf.Abs(pTracker.storedXVel) > wallJumpXForce)
        {
            wallJumpXForce = Mathf.Abs(pTracker.storedXVel);
        }

        if (Mathf.Abs(xVel) < baseMoveSpeed) { lockSpeed = true; }

        if (lockSpeed) 
        { 
            //xVel = Mathf.Clamp(xVel, -activeMoveSpeed, activeMoveSpeed); 
            if (pTracker.grounded)
                pTracker.storedXVel = 0;
        }
        
        activeMoveSpeed = baseMoveSpeed;

        if (pTracker.grounded)
        {
            if (lockSpeedTimer <= 0) 
            { 
                lockSpeed = true; 
            }

            if (Mathf.Abs(xVel) <= activeMoveSpeed)
                xVel = activeMoveSpeed * xInput;
        }
        else
        {
            if (wallJumpBoostTimer > 0 && xInput == -lastWallJump)
            {
                xVel = wallJumpXForce * -lastWallJump;
                wallJumpBoostTimer = 0;

                return;
            }


            if (xInput == 0)
            {
                xVel *= Mathf.Exp(-10f * Time.deltaTime);
            }
            else
            {
                float velToAdd = activeMoveSpeed * airMovMult * xInput * Time.deltaTime;

                if (xInput == Mathf.Sign(xVel))
                {
                    AddXVel(velToAdd, false);
                }
                else
                {
                    //AddXVel(, false);
                    xVel += activeMoveSpeed * airMovMult * 2f * Time.deltaTime * xInput;
                }
            }
        }
    }

    void JumpChecks()
    {
        int lastJumpDir = jumpDir;

        if (xInput != 0) { facingDir = xInput; }

        if (pTracker.grounded || pTracker.grabbingWall || coyoteTimer > 0) { canJump = true; }
        else { canJump = false; }

        if (pTracker.grounded)
            jumpDir = 0;

        if (jumpTimer > 0)
            jumpDir = 1;
            
        if (yVel < 0 && jumpDir == 1)
        {
            apexBoostTimer = apexBoostTime;
            jumpDir = -1;
        }

        if (jumpTimer == 0) { isJumping = false; }
        if (jumpTimer > 0) { isJumping = true; }
    }

    void JumpStart()
    {
        jumpTimer = jumpTime;

        if (pTracker.grabbingWall || coyoteTimeWall != 0)
        {
            lockSpeed = false;
            if (coyoteTimeWall != 0)
            {
                xVel = -coyoteTimeWall * wallJumpXForce;
                lastWallJump = coyoteTimeWall;
            }
            else
            {
                wallJumpBoostTimer = wallJumpBoostTime;
                xVel = -wallJumpXForce * xInput;
                lastWallJump = xInput;
            }
        }

        yVel = jumpForce;
    }

    public void JumpStop()
    {
        if (jumpDir == 1)
        {
            yVel *= 0.3f;
        }

        jumpTimer = 0;

        isJumping = false;
    }

    void GetInput()
    {
        xInput = Input.GetAxisRaw("Horizontal"); 

        if (Input.GetKeyDown(jumpKey))
        {
            if (canJump)
            {
                JumpStart();
                pTracker.jumpBufferTimer = 0;
            }
            else
                pTracker.jumpBufferTimer = pTracker.jumpBufferTime;
        }

        if (pTracker.jumpBufferTimer > 0 && canJump)
        {
            JumpStart();
        }
    }

    public void AddXVel(float addedVel, bool passesSpeedCheck)
    {
        if (Mathf.Abs(xVel + addedVel) > activeMoveSpeed)
            addedVel = 0;
        SetXVel(xVel + addedVel, true);
    }

    public void SetXVel(float newVel, bool passesSpeedCheck)
    {
        if (passesSpeedCheck)
        {
            xVel = newVel;
        }
        else
        {
            xVel = Mathf.Clamp(newVel, -activeMoveSpeed, activeMoveSpeed);
        }
    }

    void Timers()
    {
        if (jumpTimer > 0)
            jumpTimer -= Time.deltaTime;
        else
            jumpTimer = 0;
            
        if (coyoteTimer > 0)
            coyoteTimer -= Time.deltaTime;
        else
            coyoteTimer = 0;
            
        if (apexBoostTimer > 0)
            apexBoostTimer -= Time.deltaTime;
        else
            apexBoostTimer = 0;
        
        if (lockSpeedTimer > 0)
            lockSpeedTimer -= Time.deltaTime;
        else
            lockSpeedTimer = 0;
        
        if (wallJumpBoostTimer > 0)
            wallJumpBoostTimer -= Time.deltaTime;
        else
            wallJumpBoostTimer = 0;
    }

}
