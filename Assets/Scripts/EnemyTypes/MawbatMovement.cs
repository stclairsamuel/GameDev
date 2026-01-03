using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MawbatMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D col;
    private Animator anim;
    private SpriteRenderer rend;

    private EnemyBody myBody;

    private GameObject player;

    public Vector2 myPos;
    public Vector2 pPos;

    public Vector2 pDir;

    private int facingDir;

    [Header("ChaseStuff")]

    public Vector2 targetPos;

    public float flapIntervalMin;
    public float flapIntervalMax;

    public float flapModRange;

    private float flapIntervalTimer;

    public Vector2 flapForce;
    public float gravity;

    public float idealHeight;

    public float idealClearance;

    private Coroutine chaseCoroutine = null;

    float pDist;

    public float xVel;
    public float yVel;

    public int myMoveState;

    public float airDrag;

    public float sightRange;

    public float reelTime;
    float reelTimer;

    private bool isFlapping;

    public LayerMask ground;

    void OnEnable()
    {
        myBody.OnTakeDamage += OnGetHit;
        myBody.OnDeath += OnDeath;
    }
    void OnDisable()
    {
        myBody.OnTakeDamage -= OnGetHit;
        myBody.OnDeath -= OnDeath;
    }

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        myBody = GetComponent<EnemyBody>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();

        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        myPos = rb.position;
        pPos = player.GetComponent<Rigidbody2D>().position;

        pDir = (pPos - myPos);

        pDist = Vector2.Distance(pPos, myPos);

        Timers();

        Animations();

    }

    void FixedUpdate()
    {
        if (myMoveState == 0)
        {
            Wait();
        }

        if (myMoveState == 2)
        {
            Gravity();
        }

        Drag();

        rb.velocity = new Vector2(xVel, yVel);
    }

    void OnGetHit(GameObject hitBy, float dmg, Vector2 knockback)
    {
        float xKnockbackMult = 2f;

        xVel = knockback.x * xKnockbackMult;
        yVel = knockback.y;

        reelTimer = reelTime;
    }

    private void Gravity()
    {
        yVel -= gravity * Time.fixedDeltaTime;
    }

    private void Wait()
    {
        bool seePlayer = !Physics2D.Raycast(myPos, pDir, sightRange, ground);

        if (seePlayer)
        {
            SwitchStates((MoveState)2);
        }
    }

    private void StartChase()
    {
        if (chaseCoroutine == null)
            chaseCoroutine = StartCoroutine(Chase());
    }

    private IEnumerator Chase()
    {
        while (myMoveState == 2)
        {
            flapIntervalTimer = Random.Range(flapIntervalMin, flapIntervalMax);

            while (flapIntervalTimer > 0)
            {

                bool underHeight = Physics2D.BoxCast(myPos, col.bounds.size, 0, Vector2.down, idealHeight, ground);
                bool aboveClearance = Physics2D.BoxCast(myPos, col.bounds.size, 0, Vector2.up, idealClearance, ground);

                facingDir = (int)Mathf.Sign(pPos.x - myPos.x);

                float xGap = 4f;
                targetPos = new Vector2(pPos.x + (xGap * -facingDir), pPos.y + idealHeight);

                if (myPos.y < pPos.y)
                    underHeight = true;

                if (aboveClearance)
                {
                    float clearanceWaitTime = 0.5f;
                    flapIntervalTimer = clearanceWaitTime;
                    yield return null;
                }


                if (underHeight && yVel < 0)
                    flapIntervalTimer = 0;
                
                yield return null;
            }

            if (reelTimer == 0)
            {
                yVel = flapForce.y;
                xVel += flapForce.x * (targetPos - myPos).normalized.x + Random.Range(-flapModRange, flapModRange);
            }

            yield return null;
        }
    }

    private void Drag()
    {
        if (reelTimer == 0)
            xVel *= Mathf.Exp(-airDrag * Time.fixedDeltaTime);
    }

    public void SwitchStates(MoveState state)
    {
        myMoveState = (int)state;

        switch (state)
        {
            case MoveState.None:
                break;
            
            case MoveState.Idling:
                break;

            case MoveState.Chasing:
                StartChase();
                break;
        }
    }

    void OnDeath()
    {

    }

    void Animations()
    {
        anim.SetBool("isFlapping", flapIntervalTimer > 0.12f);
    }

    void Timers()
    {
        if (flapIntervalTimer > 0 && reelTimer == 0)
            flapIntervalTimer -= Time.deltaTime;
        else
            flapIntervalTimer = 0;
        
        if (reelTimer > 0)
            reelTimer -= Time.deltaTime;
        else
            reelTimer = 0;
    }

}
