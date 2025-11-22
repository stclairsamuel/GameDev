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

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = startingHealth;
        
        transform.localScale = new Vector3 (myScale.x, myScale.y, transform.localScale.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (dash.isDashing)
        {
            pMov.enabled = false;
            externalVel.x = 0;
            dash.rb.velocity = new Vector2(dash.dashForce * pMov.facingDir, 0);
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

        myPos = transform.position;

        CheckGround();

        externalVel.x *= Mathf.Exp(-0.9f * Time.deltaTime);
        if (Mathf.Abs(externalVel.x) < 0.5f) { externalVel.x = 0; }

        CheckWalls();
        
        if (Mathf.Abs(externalVel.x) > 7f)
        {
            WallCheck();
        }


        Timers();
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

    public void WallCheck()
    {
        RaycastHit2D back = Physics2D.Raycast(myPos, new Vector2(Mathf.Sign(externalVel.x), 0), transform.localScale.x * 0.5f + 0.1f, ground);
        Debug.DrawRay(transform.position, new Vector2(-Mathf.Sign(externalVel.x), 0) * (transform.localScale.x * 0.5f + 0.1f), Color.green);

        if (back)
        {
            externalVel.x *= -0.2f;
            
            if (stunTimer < 0.2f) stunTimer = 0.2f;
        }
    }

    public void CheckWalls()
    {
        float projectedX = myPos.x + ReturnXVel(pMov.xVel) * Time.deltaTime;

        Vector2 queryPos = new Vector2(Mathf.Lerp(myPos.x, projectedX, 0.5f), myPos.y);
        Vector2 queryScale = new Vector2(myScale.x + Mathf.Abs(myPos.x - projectedX), myScale.y - 0.1f);

        ContactFilter2D whatIsGround = new ContactFilter2D { layerMask = ground, useLayerMask = true };

        Collider2D[] colliders = Physics2D.OverlapBoxAll(queryPos, queryScale, 0, ground);
        bool hit = (colliders.Length > 0);

        if (hit)
        {
            Debug.Log("Hit!");

            RaycastHit2D front = Physics2D.Raycast(myPos + new Vector2(0, -myScale.y/2.2f), new Vector2(Mathf.Sign(ReturnXVel(pMov.xVel)), 0), Vector2.Distance(myPos, queryPos + new Vector2(myScale.x/2f, 0)), ground);
            if (front) 
            { 
                float newXPos = front.point.x - Mathf.Sign(ReturnXVel(pMov.xVel)) * myScale.x/2f;

                transform.position = new Vector2(newXPos, myPos.y);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        float projectedX = myPos.x + ReturnXVel(pMov.xVel) * Time.deltaTime;

        Vector2 queryPos = new Vector2(Mathf.Lerp(myPos.x, projectedX, 0.5f), myPos.y);
        Vector2 queryScale = new Vector2(myScale.x + Mathf.Abs(myPos.x - projectedX), myScale.y - 0.1f);

        Gizmos.DrawWireCube(queryPos, queryScale);
    }

    public void CheckGround()
    {
        Vector2 width = new Vector2(myScale.x/2f, 0);
        float boxBottom = transform.position.y - myScale.y/2f;

        RaycastHit2D groundLeft = Physics2D.Raycast(myPos - width + new Vector2(0.02f, 0), -Vector2.up, 100f, ground);
        RaycastHit2D groundRight = Physics2D.Raycast(myPos + width - new Vector2(0.02f, 0), -Vector2.up, 100f, ground);

        float highestGround = Mathf.Max(groundLeft.point.y, groundRight.point.y);
        float projectedY = boxBottom + pMov.yVel * Time.deltaTime;

        grounded = (projectedY <= highestGround);

        if (grounded)
        {
            transform.position = new Vector2(myPos.x, highestGround + myScale.y/2f);
            pMov.yVel = 0;
        }
        else if (!dash.isDashing)
        {
            Gravity();
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
