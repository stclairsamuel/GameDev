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

    public Vector3 myPos;

    public KeyCode jumpKey = KeyCode.Space;

    public bool canJump;
    public bool isJumping;
    public float jumpForce;
    public float jumpTime;
    public float jumpTimer;

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

    public float coyoteTime;
    public float coyoteTimer;
    public float apexBoostTime;
    public float apexBoostTimer;

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
            if (isJumping)
                JumpStop();

            if (jumpDir == 1)
            {
                yVel *= releaseJumpMultiplier;
            }
        }

        if (xInput != 0)
        {
            facingDir = xInput;
        }

        xVel = pTracker.ReturnXVel(activeMoveSpeed * xInput);
        yVel = pTracker.ReturnYVel(yVel);

        rb.velocity = new Vector2(xVel, yVel);

        Timers();
    }

    void SpeedCheck()
    {
        /*
        if (apexBoostTimer > 0)
        {
            activeMoveSpeed = apexSpeed;
        }
        else 
        {
            activeMoveSpeed = baseMoveSpeed;
        }
        */

        activeMoveSpeed = baseMoveSpeed;
    }

    void JumpChecks()
    {
        int lastJumpDir = jumpDir;

        if (xInput != 0) { facingDir = xInput; }

        if (pTracker.grounded || coyoteTimer > 0) { canJump = true; }
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

        if (Input.GetKeyDown(jumpKey) && canJump)
        {
            JumpStart();
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
    }
}
