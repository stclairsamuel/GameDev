using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RusherBase : MonoBehaviour
{
    public float gravity;

    public float activeState;

    private Transform player;
    public Vector2 pPos;

    Vector2 myPos;

    public float xVel;
    public float yVel;

    public Rigidbody2D rb;

    public RaycastHit2D hit;

    public RaycastHit2D groundCheck;
    public bool grounded;
    public LayerMask ground;

    public float dist;

    public float sightRange;

    public float chargeForce;
    public float chargeTime;
    public float chargeTimer;
    public float chargeDir;
    public float maxChargeSpeed;
    public float slowTime;

    public float stallTime;
    public float stallTimer;
    bool throwing;

    public bool chasing;

    public float lastAttack = 0;

    public float reelTime;
    bool reeling;

    public Transform throwingObject;

    public GameObject projectilePrefab;

    public float myHeight;




    // Start is called before the first frame update
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        transform.localScale = new Vector3(transform.localScale.x, myHeight, transform.localScale.z);
    }

    // Update is called once per frame
    void Update()
    {
        pPos = player.position;
        myPos = transform.position;
        
        Gravity();

        dist = Vector2.Distance(myPos, pPos);

        if (!chasing && CheckSight())
        {
            chasing = true;
        }
        if (dist > 4f * sightRange)
        {
            chasing = false;
        }

        if (reeling && stallTimer == 0)
        {
            reeling = false;
        }

        if (throwing && stallTimer == 0)
        {
            throwing = false;
            GameObject projectile = Instantiate(projectilePrefab, myPos, Quaternion.identity);
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            projectileScript.GetThrown(myPos, pPos);
        }

        if (chasing && chargeTimer == 0 && stallTimer == 0)
        {
            CheckAttack();
        }


        if (chargeTimer > 0)
        {
            Charge();
        }
        if (chargeTimer == 0)
        {
            if (reeling)
            {
                xVel += -Mathf.Sign(xVel) * 3f * Time.deltaTime;
            }
            else
            {
                xVel = 0;
            }
        }
        
        rb.velocity = new Vector2 (xVel, yVel);

        Timers();
    }

    public bool CheckSight()
    {
        if (dist < sightRange)
        {
            Transform hitTarget = Physics2D.Raycast(myPos + 0.5f * myHeight * (pPos - myPos).normalized, (pPos - myPos).normalized, sightRange).transform;
            Debug.DrawRay(2f * myPos + (pPos - myPos).normalized, (pPos - myPos).normalized * sightRange, Color.red);


            if (hitTarget == player || dist < myHeight)
            {
                return true;
            }
        }

        return false;
    }

    public void WallCheck()
    {

        RaycastHit2D front = Physics2D.Raycast(myPos - new Vector2(0, 0.25f * myHeight), new Vector2(chargeDir, 0), transform.localScale.x * 0.5f + 0.1f, ground);

        if (!front)
        {
            front = Physics2D.Raycast(myPos + new Vector2(0, 0.25f * myHeight), new Vector2(chargeDir, 0), transform.localScale.x * 0.5f + 0.1f, ground);
        }


        if (front)
        {
            transform.position = new Vector2(front.point.x - transform.localScale.x * 0.5f * chargeDir, transform.position.y);
            chargeTimer = 0;
            stallTimer = reelTime;
            reeling = true;
            xVel = chargeDir * Mathf.Abs(xVel) * -0.5f;

            if (!CheckSight() && dist > 8f)
            {
                chasing = false;
            }
        }
    }

    public void CheckAttack()
    {
        //0 = none
        //1 = charge
        //2 = ranged
        if (lastAttack == 0)
        {
            StartCharge();
        }

        else if (Random.Range(1, 3) == 1)
        {
            StartCharge();
        }
        else
        {
            StartThrow();
        }
    }

    public void StartCharge()
    {
        chargeTimer = chargeTime;
        lastAttack = 1f;
        chargeDir = Mathf.Sign((pPos - myPos).x);
    }

    public void Charge()
    {
        if (Mathf.Abs(xVel) < maxChargeSpeed && chargeTime > slowTime)
        {
            xVel += chargeForce * chargeDir * Time.deltaTime;
        }
        if (chargeTimer < slowTime)
        {
            xVel += -Mathf.Sign(xVel) * chargeForce * 2f * Time.deltaTime;
        }

        if (Mathf.Abs(xVel) > maxChargeSpeed)
        {
            xVel = maxChargeSpeed * chargeDir;
        }

        if (chargeDir != Mathf.Sign((pPos - myPos).x) && chargeTimer > 1f)
        {
            chargeTimer = slowTime;
        }

        WallCheck();
    }

    public void StartThrow()
    {
        throwing = true;
        stallTimer = stallTime;
        lastAttack = 2f;
    }


    public void Gravity()
    {
        grounded = Physics2D.Raycast(myPos, -Vector2.up, myHeight * 0.5f + 0.1f, ground);
        if (!grounded)
        {
            yVel -= gravity * Time.deltaTime;
        }
        else if (yVel < 0)
        {
            yVel = 0;
            groundCheck = Physics2D.Raycast(myPos, -Vector2.up, myHeight * 0.5f + 0.1f, ground);
            transform.position = new Vector2(transform.position.x, groundCheck.point.y + myHeight * 0.5f);
        }
    }

    void Timers()
    {
        if (chargeTimer > 0)
        {
            chargeTimer -= Time.deltaTime;
        }
        else
        {
            chargeTimer = 0;
        }        
        if (stallTimer > 0)
        {
            stallTimer -= Time.deltaTime;
        }
        else
        {
            stallTimer = 0;
        }
    }
}
