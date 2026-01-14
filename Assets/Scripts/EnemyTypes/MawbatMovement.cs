using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MawbatMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D col;
    private Animator anim;
    private SpriteRenderer rend;

    public GameObject biteHitbox;

    private EnemyBody myBody;

    private GameObject player;

    public Vector2 myPos;
    public Vector2 pPos;

    public Vector2 pDir;
    public Rigidbody2D pRb;

    public int facingDir;

    private bool readyingAttack;

    [Header("ChaseStuff")]

    public Vector2 targetPos;

    public float flyingXGap;
    public float attackingXGap;

    public float flapIntervalMin;
    public float flapIntervalMax;

    public float flapModRange;

    private float flapIntervalTimer;

    public Vector2 flapForce;
    public float gravity;

    public float idealFlyingHeight;
    public float idealAttackHeight;

    public float idealClearance;

    private Coroutine chaseCoroutine = null;


    [Header("AttackStuff")]

    public float readyIntervalMax;
    public float readyIntervalMin;

    public float attackForce;

    public float attackTime;

    public float attackCDIntervalMax;
    public float attackCDIntervalMin;
    private float attackCDTimer;

    private Coroutine attackingCoroutine = null;

    float pDist;

    public float xVel;
    public float yVel;

    public int myMoveState;

    public float airDrag;

    public float sightRange;

    public float reelTime;
    float reelTimer;

    private bool dying;

    private bool isFlapping;

    public LayerMask ground;

    public GameObject deathExplosion;

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
        myMoveState = 0;

        facingDir = 1;

        rb = GetComponent<Rigidbody2D>();
        myBody = GetComponent<EnemyBody>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();

        player = GameObject.FindWithTag("Player");
        pRb = player.GetComponent<Rigidbody2D>();

        pPos = pRb.position;
        myPos = rb.position;

        if (transform.parent != null)
        {
            // Access the parent GameObject's tag
            if (transform.parent.CompareTag("Node"))
            {
                StartCoroutine(SpawnCycle());
            }
        }
    }

    private IEnumerator SpawnCycle()
    {
        myMoveState = 0;
        sightRange = Mathf.Infinity;
        col.enabled = false;
        rb.simulated = false;

        yield return new WaitForSeconds(1f);

        col.enabled = true;
        rb.simulated = true;

        yVel = 0;

        SwitchStates((MoveState)2);
    }

    // Update is called once per frame
    void Update()
    {
        myPos = rb.position;
        pPos = pRb.position;

        transform.localScale = new Vector3(-facingDir, 1, 0);

        pDir = (pPos - myPos);

        pDist = Vector2.Distance(pPos, myPos);

        if (pDist < 6f && attackingCoroutine == null && attackCDTimer == 0 && myMoveState == 2)
        {
            attackingCoroutine = StartCoroutine(ReadyAttack());
        }

        if (readyingAttack && pDist > 6f)
        {
            SwitchStates((MoveState)2);
        }

        if (myBody.currentHealth <= 0)
            myMoveState = -1;

        biteHitbox.SetActive(myMoveState == 3);

        if (dying)
        {
            if (Physics2D.BoxCast(myPos, col.bounds.size * 0.8f, 0, Vector2.down, 0.2f, ground))
            {
                Instantiate(deathExplosion, myPos, Quaternion.identity);
                Destroy(gameObject);
            }
        }

        Timers();

        Animations();

    }

    void FixedUpdate()
    {

        
        if (myMoveState == 0)
        {
            Wait();
        }

        if (myMoveState == 2 || myMoveState == -1)
        {
            Gravity();
        }

        Drag();

        rb.velocity = new Vector2(xVel, yVel);
    }

    void OnGetHit(DamageInfo info)
    {
        float xKnockbackMult = 2f;

        xVel = info.Knockback.x * xKnockbackMult;
        yVel = info.Knockback.y;

        reelTimer = info.StunTime;

        if (attackingCoroutine != null)
        {
            SwitchStates((MoveState)2);
        }
    }

    private IEnumerator ReadyAttack()
    {
        readyingAttack = true;

        yield return new WaitForSeconds(Random.Range(readyIntervalMin, readyIntervalMax));
        yield return new WaitUntil(() => (Mathf.Abs(myPos.y - pPos.y) < 0.4f && pDist < 2f));

        SwitchStates((MoveState)3);

        readyingAttack = false;
        
        xVel = facingDir * attackForce;
        yVel = 0;

        yield return new WaitForSeconds(attackTime);
        attackCDTimer = Random.Range(attackCDIntervalMin, attackCDIntervalMax);

        SwitchStates((MoveState)2);
    }

    private void Gravity()
    {
        yVel -= gravity * Time.fixedDeltaTime;
    }

    private void Wait()
    {
        pDist = Vector2.Distance(pPos, myPos);

        bool seePlayer = !Physics2D.Raycast(myPos, pDir, pDist, ground) && pDist < sightRange;

        if (seePlayer)
        {
            SwitchStates((MoveState)2);
        }

        RaycastHit2D topCheck = Physics2D.Raycast(myPos, Vector2.up, 100f, ground);
        float halfHeight = col.bounds.size.y/2f;

        if (topCheck)
        {
            //transform.position = new Vector2(myPos.x, topCheck.point.y - halfHeight);
        }
    }

    private void StartChase()
    {
        if (attackingCoroutine != null)
        {
            attackCDTimer = Random.Range(attackCDIntervalMin, attackCDIntervalMax);
            StopCoroutine(attackingCoroutine);
            attackingCoroutine = null;
            readyingAttack = false;
        }

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
                float idealHeight = readyingAttack ? idealAttackHeight : idealFlyingHeight;

                bool underHeight = Physics2D.BoxCast(myPos, col.bounds.size, 0, Vector2.down, idealHeight, ground);
                bool aboveClearance = Physics2D.BoxCast(myPos, col.bounds.size, 0, Vector2.up, idealClearance, ground);

                facingDir = (int)Mathf.Sign(pPos.x - myPos.x);

                float xGap = readyingAttack ? attackingXGap : flyingXGap;
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

    private void StartBite()
    {
        if (chaseCoroutine != null)
        {
            StopCoroutine(chaseCoroutine);
            chaseCoroutine = null;
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
            
            case MoveState.Striking:
                StartBite();
                break;
        }
    }

    void OnDeath(GameObject hitBy, float damage, Vector2 knockback)
    {
        SwitchStates((MoveState)(-1));

        float knockbackMult = 6f;

        xVel = knockback.x * knockbackMult;
        yVel = knockback.y * knockbackMult/4f;

        if (chaseCoroutine != null)
        {
            StopCoroutine(chaseCoroutine);
            chaseCoroutine = null;
        }
        if (attackingCoroutine != null)
        {
            attackCDTimer = Random.Range(attackCDIntervalMin, attackCDIntervalMax);
            StopCoroutine(attackingCoroutine);
            attackingCoroutine = null;
            readyingAttack = false;
        }

        dying = true;
    }

    void Animations()
    {
        anim.SetBool("isFlapping", flapIntervalTimer > 0.125f);
        anim.SetBool("isBiting", myMoveState == 3);
        anim.SetBool("isHurting", reelTimer > 0 || dying);
        anim.SetBool("isSleeping", myMoveState == 0);

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

        if (attackCDTimer > 0)
            attackCDTimer -= Time.deltaTime;
        else
            attackCDTimer = 0;
    }

}
