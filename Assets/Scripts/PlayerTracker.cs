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

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = startingHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (dash.isDashing)
        {
            pMov.enabled = false;
            externalVel.x = 0;
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

        externalVel.x *= Mathf.Exp(-0.9f * Time.deltaTime);
        if (Mathf.Abs(externalVel.x) < 0.5f) { externalVel.x = 0; }
        
        if (Mathf.Abs(externalVel.x) > 7f)
        {
            WallCheck();
        }


        Timers();
    }

    public void WallCheck()
    {
        RaycastHit2D back = Physics2D.Raycast(myPos, new Vector2(Mathf.Sign(externalVel.x), 0), transform.localScale.x * 0.5f + 0.1f, pMov.ground);
        Debug.DrawRay(transform.position, new Vector2(-Mathf.Sign(externalVel.x), 0) * (transform.localScale.x * 0.5f + 0.1f), Color.green);

        if (back)
        {
            externalVel.x *= -0.2f;
            
            if (stunTimer < 0.2f) stunTimer = 0.2f;
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
