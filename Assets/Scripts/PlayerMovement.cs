using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
//test

    public float gravity;

    public Rigidbody2D rb;

    public float xVel;
    public float yVel;
    
    public float baseMoveSpeed;
    public float activeMoveSpeed;

    public Vector3 myPos;

    public KeyCode jumpKey = KeyCode.Space;

    public bool grounded;
    public LayerMask ground;
    public RaycastHit2D groundCheck;

    public bool canJump;
    public bool isJumping;
    public float jumpForce;
    public float jumpTime;
    public float jumpTimer;

    public float xInput;
    public float externalXVel;

    public float facingDir;

    public bool takeInput;

    public PlayerTracker pTracker;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        activeMoveSpeed = baseMoveSpeed;

        if (xInput != 0) { facingDir = xInput; }

        myPos = transform.position;
        grounded = Physics2D.Raycast(myPos, -Vector3.up, 0.6f, ground);

        Gravity();
        
        //jump start

        if (grounded) { canJump = true; }
        else { canJump = false; }

        if (jumpTimer == 0) { isJumping = false; }
        if (jumpTimer > 0) { isJumping = true; }

        if (takeInput)
        {
            GetInput();
        }
        else
        {
            xInput = 0;
        }

        if (Input.GetKeyUp(jumpKey) && isJumping)
        {
            JumpStop();
        }

        //jump end

        if (xInput != 0)
        {
            facingDir = xInput;
        }

        xVel = pTracker.ReturnXVel(activeMoveSpeed * xInput);

        rb.velocity = new Vector2(xVel, yVel);

        Timers();
    }

    void JumpStart()
    {
        jumpTimer = jumpTime;
        yVel = jumpForce;
    }
    void JumpStop()
    {
        if (yVel > 0)
        {
            yVel *= 0.3f;
        }

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

    void Gravity()
    {
        if (!grounded)
        {
            yVel -= gravity * Time.deltaTime;
        }
        else if (yVel < 0)
        {
            yVel = -2f;
        }
    }



    void Timers()
    {
        if (jumpTimer > 0)
        {
            jumpTimer -= Time.deltaTime;
        }
        else
        {
            jumpTimer = 0;
        }
    }
}
