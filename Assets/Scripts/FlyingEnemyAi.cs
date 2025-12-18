using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemyAi : MonoBehaviour
{
    public float xVel;
    public float yVel;

    public float flyingAcceleration;
    public float maxFlySpeed;
    public float maxDiveSpeed;
    public float slowMult;

    public float facingDir;

    public bool isFlying;
    public bool isDiving;
    public bool isAttacking;
    public bool isSwitching;

    private Rigidbody2D rb;
    private Collider2D contact;

    public Vector2 myPos;
    public Vector2 myScale;

    private GameObject player;
    private PlayerTracker pTracker;
    public Vector2 pPos;

    public float idealGapX;
    public float idealGapY;
    public float slackX;
    public float slackY;
    public float playerDist;
    public float minHeight;
    public float diveHeight;
    public float minClearance;

    public LayerMask ground;

    public RaycastHit2D clearanceCheck;
    public RaycastHit2D groundCheck;
    public RaycastHit2D backCheck;
    public RaycastHit2D frontCheck;

    float clearance;
    float height;

    public float switchTime;
    public float switchTimer;

    public float diveCDTime;
    public float diveCDTimer;

    //private EnemyBody myBody;

    // Start is called before the first frame update
    void Start()
    {
        diveCDTimer = diveCDTime;

        rb = GetComponent<Rigidbody2D>();
        contact = GetComponent<Collider2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        pTracker = player.GetComponent<PlayerTracker>();
        //myBody = GetComponent<EnemyBody>();
    }

    // Update is called once per frame
    void Update()
    {
        myPos = transform.position;
        pPos = player.transform.position;

        playerDist = Vector2.Distance(myPos, pPos);

        isFlying = !isDiving;

        if (diveCDTimer == 0 && isFlying && pTracker.grounded)
            StartDive();

        if (isFlying)
        {
            Move();
        }
        if (isDiving)
        {
            Dive();
        }
        if (isAttacking)
        {
            Attack();
        }


        rb.velocity = new Vector2(xVel, yVel);

        Timers();
    }

    void Move()
    {
        Vector2 dir = pPos - myPos;
        float myHeight = myPos.y;
        float pHeight = pPos.y;
        float yGap = Mathf.Abs(myHeight - pHeight);
        float xGap = Mathf.Abs(myPos.x - pPos.x);
        
        if (!isSwitching)
            facingDir = Mathf.Sign(pPos.x - myPos.x);

        groundCheck = Physics2D.BoxCast(myPos, myScale * 0.5f, 0, Vector2.down, myScale.y/2f + minHeight, ground);
        clearanceCheck = Physics2D.BoxCast(myPos, myScale * 0.5f, 0, Vector2.up, myScale.y/2f + minClearance, ground);

        bool idealHeight = !groundCheck;
        bool idealClearance = !clearanceCheck;

        Vector2 flyDir = Vector2.zero;

        if (yGap < idealGapY + slackY || Mathf.Sign(myPos.y - pPos.y) == -1 || groundCheck)
        {
            flyDir = new Vector2(flyDir.x, 1);
        }
        if ((yGap > idealGapY - slackY && Mathf.Sign(myPos.y - pPos.y) == 1) || clearanceCheck)
        {
            flyDir = new Vector2(flyDir.x, -1);
        }
        if (!groundCheck && !clearanceCheck && yGap < idealGapY + slackY && yGap > idealGapY - slackY)
        {
            flyDir = new Vector2(flyDir.x, 0);
        }

        if (isSwitching)
        {
            xVel = Mathf.Clamp(xVel + facingDir * flyingAcceleration * Time.deltaTime, -maxFlySpeed * 2f, maxFlySpeed * 2f);
            yVel = flyDir.y != 0 ? Mathf.Clamp(yVel + flyDir.y * flyingAcceleration * Time.deltaTime, -maxFlySpeed, maxFlySpeed)
                : yVel * Mathf.Exp(-slowMult * Time.deltaTime);
            if (xGap > idealGapX + slackX)
                switchTimer = 0;
            if (switchTimer == 0)
                isSwitching = false;
            return;
        }

        if (xGap < idealGapX + slackX)
        {
            flyDir = new Vector2(-facingDir, flyDir.y);
        }
        if (xGap > idealGapX - slackX)
        {
            flyDir = new Vector2(facingDir, flyDir.y);
        }
        if (xGap < idealGapX + slackX && xGap > idealGapX - slackX)
        {
            flyDir = new Vector2(0, flyDir.y);
        }

        backCheck = Physics2D.BoxCast(myPos, myScale/2f, 0, new Vector2(-facingDir, 0), myScale.x/2f + 0.1f, ground);

        if (backCheck && xGap < idealGapX)
        {
            isSwitching = true;
            switchTimer = switchTime;
        }

        xVel = flyDir.x != 0 ? Mathf.Clamp(xVel + flyDir.x * flyingAcceleration * Time.deltaTime, -maxFlySpeed, maxFlySpeed)
            : xVel * Mathf.Exp(-slowMult * Time.deltaTime);
        yVel = flyDir.y != 0 ? Mathf.Clamp(yVel + flyDir.y * flyingAcceleration * Time.deltaTime, -maxFlySpeed, maxFlySpeed)
            : yVel * Mathf.Exp(-slowMult * Time.deltaTime);
    }

    void StartDive()
    {
        isDiving = true;
        isFlying = false;
        yVel = 0;
        xVel = 0;
    }

    void EndDive()
    {
        diveCDTimer = diveCDTime;
        isDiving = false;
    }

    void Dive()
    {
        float xGap = Mathf.Abs(myPos.x - pPos.x);

        if (xGap > idealGapX + slackX)
        {
            EndDive();
            return;
        }

        groundCheck = Physics2D.BoxCast(myPos, myScale/2f, 0, Vector2.down, diveHeight + myScale.y/2f + 0.1f, ground);

        xVel = Mathf.Clamp(xVel + flyingAcceleration * facingDir * Time.deltaTime, -maxDiveSpeed, maxDiveSpeed);
        yVel = !groundCheck ? Mathf.Clamp(yVel - flyingAcceleration * Time.deltaTime, -maxDiveSpeed, maxDiveSpeed)
            : yVel * Mathf.Exp(-slowMult * Time.deltaTime);
    }

    void Attack()
    {

    }

    void OnTriggerEnter2D(Collider2D touching)
    {
        
    }

    void Timers()
    {

        if (switchTimer > 0)
            switchTimer -= Time.deltaTime;
        else
            switchTimer = 0;
        
        if (diveCDTimer > 0)
            diveCDTimer -= Time.deltaTime;
        else
            diveCDTimer = 0;
    }
}
