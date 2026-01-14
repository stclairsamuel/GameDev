using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2 : MonoBehaviour
{
    private PlayerTracker2 myTracker;

    private Rigidbody2D rb;

    public float gravity;
    public float fastFallMult;

    public float jumpForce;
    public bool isJumping;

    public float acceleration;
    public float moveSpeed;

    public float groundDrag;
    public float groundIdleDrag;
    public float airDrag;
    public float airIdleDrag;

    private bool lockSpeed = true;

    public float xVel;
    public float yVel;

    private bool fastFall;

    public float dashForce;

    private bool grounded;

    private float xInput;

    void OnEnable()
    {
        myTracker.OnGroundTouch += GroundTouch;
    }
    void OnDisable()
    {
        myTracker.OnGroundTouch -= GroundTouch;
    }

    void Awake()
    {
        myTracker = GetComponent<PlayerTracker2>();

        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        grounded = myTracker.grounded;
    }

    void FixedUpdate()
    {
        Gravity();

        Drag();

        rb.velocity = new Vector2(xVel, yVel);
    }

    public void StartJump()
    {
        isJumping = true;
        yVel = jumpForce;
    }
    public void StopJump()
    {
        isJumping = false;
        yVel *= 0.3f;
        fastFall = true;
    }

    public void StartDash()
    {
        lockSpeed = false;
        xVel = dashForce * myTracker.facingDir;
    }

    public void Move()
    {
        if (Mathf.Abs(xVel) < moveSpeed)
            xVel += acceleration * myTracker.facingDir * Time.fixedDeltaTime;

        if (Mathf.Abs(xVel) > moveSpeed && lockSpeed)
            xVel = Mathf.Clamp(xVel, -moveSpeed, moveSpeed);
    }

    public void Drag()
    {
        int xInput = (int)Input.GetAxisRaw("Horizontal");

        float dragToUse = 0;

        if (grounded)
        {
            if (xInput == 0)
                dragToUse = groundIdleDrag;
            else if (Mathf.Abs(xVel) > moveSpeed)
                dragToUse = groundDrag;
        }
        else
        {
            if (xInput == 0)
                dragToUse = airIdleDrag;
            else if (Mathf.Abs(xVel) > moveSpeed)
                dragToUse = airDrag;
        }

        xVel *= Mathf.Exp(-dragToUse * Time.fixedDeltaTime);
    }

    void Gravity()
    {
        if (!grounded)
        {
            yVel -= (fastFall ? gravity * Time.fixedDeltaTime * fastFallMult : gravity * Time.fixedDeltaTime);
        }
        else if (grounded && yVel < 0)
        {
            yVel = -2f;
        }

    }

    void GroundTouch()
    {
        fastFall = false;
    }
}
