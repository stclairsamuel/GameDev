using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
//test123

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

    public Vector2 myScale;

    public GameObject colliderLeft;
    public GameObject colliderRight;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3 (myScale.x, myScale.y, transform.localScale.z);
    }

    // Update is called once per frame
    void Update()
    {
        myPos = transform.position;

        activeMoveSpeed = baseMoveSpeed;

        if (xInput != 0) { facingDir = xInput; }

        CheckGround();
        
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
        yVel -= gravity * Time.deltaTime;
    }

    void CheckGround()
    {
        Vector3 width = new Vector3(myScale.x/2f, 0, 0);
        float boxBottom = transform.position.y - myScale.y/2f;

        RaycastHit2D groundLeft = Physics2D.Raycast(myPos - width, -Vector2.up, 100f, ground);
        RaycastHit2D groundRight = Physics2D.Raycast(myPos + width, -Vector2.up, 100f, ground);

        float highestGround = Mathf.Max(groundLeft.point.y, groundRight.point.y);
        float projectedY = boxBottom + yVel * Time.deltaTime;

        Debug.Log("Highest Ground: " + highestGround.ToString());
        Debug.Log("Projected Y: " + (projectedY).ToString());

        grounded = (projectedY <= highestGround);

        if (grounded)
        {
            transform.position = new Vector3(myPos.x, highestGround + myScale.y/2f, myPos.z);
            yVel = 0;
        }
        else
        {
            Gravity();
        }
    }

    void CheckWalls()
    {
        ColliderCheck right = colliderRight.GetComponent<ColliderCheck>();
        ColliderCheck left = colliderLeft.GetComponent<ColliderCheck>();


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
