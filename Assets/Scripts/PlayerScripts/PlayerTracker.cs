using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class PlayerTracker : MonoBehaviour
{
    public event Action OnPlayerDamaged;

    public event Action OnGroundContact;
    public event Action OnWallContact;

    public float startingHealth;
    public float currentHealth;

    public TimeStop timeStop;

    public PlayerMovement pMov;
    public Dash dash;

    public Vector2 myPos;

    float stunTimer;

    public Vector2 externalVel;

    public Vector3 myScale;

    public float gravity;

    public bool grounded;

    public LayerMask ground;

    public bool touchingLeft;
    public bool touchingRight;
    public bool grabbingWall;
    public float wallDrag;
    public float wallSlideSpeed;

    float predictedX;
    float predictedY;

    float halfHeight;
    float halfWidth;

    public float jumpBufferTime;
    public float jumpBufferTimer;

    public float dashWallJumpTime;
    public float dashWallJumptimer;

    public float storedXVel;
    public float storedVelTime;
    public float storedVelTimer;

    private List<RaycastHit2D> groundChecks = new List<RaycastHit2D>() { 
        new RaycastHit2D(),
        new RaycastHit2D(),
        new RaycastHit2D()
    };
    private List<RaycastHit2D> topChecks = new List<RaycastHit2D>() { 
        new RaycastHit2D(),
        new RaycastHit2D(),
        new RaycastHit2D()
    };
    private List<RaycastHit2D> leftChecks = new List<RaycastHit2D>() { 
        new RaycastHit2D(),
        new RaycastHit2D(),
        new RaycastHit2D()
    };
    private List<RaycastHit2D> rightChecks = new List<RaycastHit2D>() { 
        new RaycastHit2D(),
        new RaycastHit2D(),
        new RaycastHit2D()
    };

    private void OnEnable()
    {
        OnGroundContact += GroundTouch;
    }
    private void OnDisable()
    {
        OnGroundContact -= GroundTouch;
    }

    // Start is called before the first frame update
    private void Awake()
    {
        
        transform.localScale = new Vector3 (myScale.x, myScale.y, transform.localScale.z);
        
        halfWidth = myScale.x/2f;
        halfHeight = myScale.y/2f;
    }

    // Update is called once per frame
    void Update()
    {
        myPos = transform.position;

        predictedX = myPos.x + ReturnXVel(pMov.xVel) * Time.deltaTime;
        predictedY = myPos.y + ReturnYVel(pMov.yVel) * Time.deltaTime;

        TestPMov();

        CheckGround();
        CheckTop();

        externalVel.x *= Mathf.Exp(-0.9f * Time.deltaTime);
        if (Mathf.Abs(externalVel.x) < 0.5f) { externalVel.x = 0; }
        
        CheckWalls();
        
        Timers();

        if (currentHealth <= 0)
        {
            SceneManager.LoadScene("DeathScreen");
        }
    }

    void TestPMov()
    {
        if (dash.isDashing)
        {
            pMov.enabled = false;
            externalVel.x = 0;
            dash.rb.velocity = new Vector2(dash.dashForce * pMov.facingDir, 0);
            pMov.coyoteTimer = 0;
        }
        else
        {
            pMov.enabled = true;
        }

        if (stunTimer > 0)
        {
            pMov.takeInput = false;
        }
        else
        {
            pMov.takeInput = true; 
        }
    }

    public float ReturnXVel(float xMov)
    {
        if (stunTimer > 0)
        {
            return externalVel.x;
        }
        if (Mathf.Abs(externalVel.x) > Mathf.Abs(xMov))
        {
            if (pMov.xInput == -Mathf.Sign(externalVel.x))
            {
                externalVel.x += pMov.xInput * pMov.activeMoveSpeed * Time.deltaTime;
            }

            return externalVel.x;
        }
        if (Mathf.Abs(externalVel.x) < Mathf.Abs(xMov))
        {
            if (pMov.xInput != 0)
            {
                externalVel.x = 0;
            }
        }
        
        return xMov;
    }

    public float ReturnYVel(float yVel)
    {
        return yVel;
    }

    public void CheckWalls()
    {
        RaycastHit2D leftCast = Physics2D.BoxCast(myPos, new Vector2(myScale.x, 0.2f), 0, Vector2.left, 0.1f, ground);
        RaycastHit2D rightCast = Physics2D.BoxCast(myPos, new Vector2(myScale.x, 0.2f), 0, Vector2.right, 0.1f, ground);

        float offSet = Mathf.Sign(ReturnXVel(pMov.xVel)) * halfWidth + 0.2f * Mathf.Sign(ReturnXVel(pMov.xVel));

        RaycastHit2D topForwards = Physics2D.Raycast(new Vector2(myPos.x, myPos.y + halfHeight/2f), new Vector2(Mathf.Sign(ReturnXVel(pMov.xVel)), 0), halfWidth + 0.2f, ground);
        RaycastHit2D bottomForwards = Physics2D.Raycast(new Vector2(myPos.x, myPos.y - halfHeight/2f), new Vector2(Mathf.Sign(ReturnXVel(pMov.xVel)), 0), halfWidth + 0.2f, ground);

        RaycastHit2D topClipping = Physics2D.Raycast(new Vector2(myPos.x + offSet, myPos.y + halfHeight/2f), Vector2.up, halfHeight/2f, ground);
        RaycastHit2D bottomClipping = Physics2D.Raycast(new Vector2(myPos.x + offSet, myPos.y - halfHeight/2f), Vector2.down, halfHeight/2f, ground);

        if (!topForwards && topClipping)
        {
            if (Mathf.Abs(ReturnXVel(pMov.xVel)) > pMov.baseMoveSpeed || dash.isDashing)
                SetPos(new Vector2(myPos.x, myPos.y - halfHeight/2f + topClipping.distance - 0.02f));
        }

        bool leftLastFrame = touchingLeft;
        bool rightLastFrame = touchingRight;

        touchingLeft = leftCast;
        touchingRight = rightCast;

        if (touchingLeft && !leftLastFrame || touchingRight && !rightLastFrame)
        {
            if (Mathf.Abs(pMov.xVel) > Mathf.Abs(storedXVel))
            {
                storedXVel = pMov.xVel;
            }
            storedVelTimer = storedVelTime;
        }

        if (pMov.xInput == -1 && touchingLeft || pMov.xInput == 1 && touchingRight)
        {
            grabbingWall = true;
        }
        else
        {
            grabbingWall = false;
        }

        if (Mathf.Sign(ReturnXVel(pMov.xVel)) == -1 && touchingLeft || Mathf.Sign(ReturnXVel(pMov.xVel)) == 1 && touchingRight) { pMov.xVel = 0; }

        if (touchingLeft && !grounded && pMov.xInput != -1)
        {
            pMov.coyoteTimer = pMov.coyoteTime;
            pMov.coyoteTimeWall = -1;
        }
        if (touchingRight && !grounded && pMov.xInput != 1)
        {
            pMov.coyoteTimer = pMov.coyoteTime;
            pMov.coyoteTimeWall = 1;
        }
        if (pMov.coyoteTimer == 0)
        {
            pMov.coyoteTimeWall = 0;
        }

        if (Mathf.Abs(externalVel.x) > 7f)
        {
            if ((externalVel.x < 0 && touchingLeft) || (externalVel.x > 0 && touchingLeft))
            {
                externalVel.x *= -0.2f;
            }
        }
    }

    public void CheckGround()
    {
        bool groundedLastFrame = grounded;

        grounded = Physics2D.BoxCast(myPos - new Vector2(0, halfHeight), new Vector2(myScale.x, 0.05f), 0, Vector2.down, 0.1f, ground);

        if (grounded && !groundedLastFrame)
        {
            OnGroundContact?.Invoke();
        }

        if (grounded && pMov.yVel < 0)
        {
            if (!groundedLastFrame && Mathf.Abs(pMov.xVel) > pMov.baseMoveSpeed)
            {
                    pMov.lockSpeedTimer = pMov.lockSpeedTime;
            }
            pMov.yVel = -2f;
        }
        else if (grabbingWall && pMov.yVel <= 0)
        {
            if (pMov.yVel < -wallSlideSpeed)
                pMov.yVel *= Mathf.Exp(-wallDrag * Time.deltaTime);
            else
                pMov.yVel = -wallSlideSpeed;
        }
        else
        {
            if (groundedLastFrame && (pMov.jumpTimer == 0))
            {
                pMov.coyoteTimer = pMov.coyoteTime;
            }
                
            Gravity();
        }
    }

    public void CheckTop()
    {
        RaycastHit2D groundTest = Physics2D.BoxCast(myPos + new Vector2(0, halfHeight), new Vector2(myScale.x, 0.05f), 0, Vector2.up, 0.1f, ground);

        if (groundTest && pMov.yVel > 0)
        {
            float[] topHeights = new float[3];

            for (int i = 0; i < topChecks.Count; i++)
            {
                Vector2 offSet = new Vector2((-halfWidth + i * halfWidth), 0);
                topChecks[i] = Physics2D.Raycast(myPos + offSet, Vector2.up, 100f, ground);
                topHeights[i] = topChecks[i].collider ? myPos.y + topChecks[i].distance : Mathf.Infinity;
            }

            bool left = topHeights[0] - myPos.y < halfHeight + 0.2f;
            bool mid = topHeights[1] - myPos.y < halfHeight + 0.2f;
            bool right = topHeights[2] - myPos.y < halfHeight + 0.2f;

            if (left && !mid && !right)
            {
                RaycastHit2D ray = Physics2D.Raycast(myPos + new Vector2(0, halfHeight + 0.2f), Vector2.left, halfWidth, ground);
                float correctedX = myPos.x + (halfWidth - ray.distance + 0.02f);
                SetPos(new Vector2(correctedX, myPos.y));
            }
            else if (right && !mid && !left)
            {
                RaycastHit2D ray = Physics2D.Raycast(myPos + new Vector2(0, halfHeight + 0.2f), Vector2.right, halfWidth, ground);
                float correctedX = myPos.x - (halfWidth - ray.distance + 0.02f);
                SetPos(new Vector2(correctedX, myPos.y));
            }

            else
            {
                pMov.yVel = -0.02f;
                if (pMov.isJumping)
                {
                    pMov.JumpStop();
                }
            }

        }
    }

    void Gravity()
    {
        pMov.yVel -= gravity * Time.deltaTime;
        pMov.yVel = Mathf.Clamp(pMov.yVel, -20f, Mathf.Infinity);
    }

    public void Damage(GameObject hitBy, float damageAmt, Vector2 knockback, float freezeTime = 0.2f, float stunTime = 0.3f)
    {
        pMov.coyoteTimer = 0;
        if (dash.isDashing)
        {
            dash.DashEnd();
        }
        else
        {
            dash.dashCooldownTimer = 0.1f;
        }

        externalVel.x = knockback.x;
        pMov.yVel = knockback.y;

        timeStop.RequestFreeze(freezeTime);

        stunTimer = stunTime;
        
        currentHealth -= damageAmt;

        OnPlayerDamaged?.Invoke();
    }

    public void SetPos(Vector2 newPos)
    {
        transform.position = newPos;
        myPos = newPos;
        predictedX = newPos.x + ReturnXVel(pMov.xVel) * Time.deltaTime;
        predictedY = newPos.y + ReturnYVel(pMov.yVel) * Time.deltaTime;
    }

    void Timers()
    {
        if (stunTimer > 0)
        {
            stunTimer -= Time.deltaTime;  
        }
        else
        {
            stunTimer = 0;
        }

        if (jumpBufferTimer > 0)
        {
            jumpBufferTimer -= Time.deltaTime;
        }
        else
        {
            jumpBufferTimer = 0;
        }

        if (dashWallJumptimer > 0)
        {
            dashWallJumptimer -= Time.deltaTime;
        }
        else
        {
            dashWallJumptimer = 0;
        }

        if (storedVelTimer > 0)
        {
            storedVelTimer -= Time.deltaTime;
        }
        else
        {
            storedVelTimer = 0;
        }
    }

    private void GroundTouch()
    {
        if (pMov.xVel > pMov.baseMoveSpeed)
        {
            pMov.lockSpeedTimer = pMov.lockSpeedTime;
        }
    }
}
