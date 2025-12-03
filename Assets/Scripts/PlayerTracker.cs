using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTracker : MonoBehaviour
{
    public float startingHealth;
    public float currentHealth;

    public TimeStop timeStop;

    public PlayerMovement pMov;
    public Dash dash;

    Vector2 myPos;

    float stunTimer;

    public Vector2 externalVel;

    public Vector3 myScale;

    public float gravity;

    public bool grounded;

    public LayerMask ground;

    public bool touchingLeft;
    public bool touchingRight;

    float predictedX;
    float predictedY;

    float halfHeight;
    float halfWidth;

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


    // Start is called before the first frame update
    void Start()
    {
        currentHealth = startingHealth;
        
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
        
        //CheckWalls();
        
        Timers();
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

    public float ReturnXVel(float xVel)
    {
        if (stunTimer > 0)
        {
            return externalVel.x;
        }
        if (Mathf.Abs(externalVel.x) > Mathf.Abs(xVel))
        {
            if (pMov.xInput == -Mathf.Sign(externalVel.x))
            {
                externalVel.x += pMov.xInput * pMov.activeMoveSpeed * Time.deltaTime;
            }

            return externalVel.x;
        }
        if (Mathf.Abs(externalVel.x) < Mathf.Abs(xVel))
        {
            if (pMov.xInput != 0)
            {
                externalVel.x = 0;
            }
        }
        
        return xVel;
    }

    public float ReturnYVel(float yVel)
    {
        return yVel;
    }

    public void CheckWalls()
    {
        RaycastHit2D leftCast = Physics2D.BoxCast(myPos, myScale, 0, Vector2.left, 0.1f, ground);
        RaycastHit2D rightCast = Physics2D.BoxCast(myPos, myScale, 0, Vector2.right, 0.1f, ground);

        touchingLeft = leftCast;
        touchingRight = rightCast;

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

        if (grounded && pMov.yVel < 0)
            pMov.yVel = -2f;
        else
            if (groundedLastFrame && (pMov.jumpTimer == 0))
            {
                pMov.coyoteTimer = pMov.coyoteTime;
            }
                
            Gravity();
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
    }

    public void Damage(GameObject hitBy, float damageAmt, float knockback, float freezeTime = 0.2f, float stunTime = 0.3f)
    {
        if (dash.isDashing)
        {
            dash.DashEnd();
        }
        else
        {
            dash.dashCooldownTimer = 0.1f;
        }

        externalVel.x = knockback;
        pMov.yVel = 5f;

        timeStop.RequestFreeze(freezeTime);

        stunTimer = stunTime;
        
        currentHealth -= damageAmt;
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
    }
}
