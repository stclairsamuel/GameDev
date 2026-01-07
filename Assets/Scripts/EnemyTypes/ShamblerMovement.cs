using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShamblerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D col;
    private Animator anim;
    private SpriteRenderer rend;

    private EnemyBody myBody;

    private GameObject player;
    private Vector2 pPos;

    public float gravity;

    private float pDist;

    public float xVel;
    public float yVel;

    public Vector2 lungeForce;
    public bool canLunge;
    public float lungeRange;
    public float lungeCDTime;
    private float lungeCDTimer;
    private bool isLunging;
    public int lungeStep;
    public float crouchTime;

    bool grounded;
    public RaycastHit2D groundCheck;
    public LayerMask ground;
    public float groundDrag;
    public float airDrag;

    private Vector2 myPos;

    public float walkSpeed;
    public int facingDir;
    public float maxChaseSpeed;
    public float chaseAccel;

    public float walkTime;
    public float walkTimer;
    public float waitTime;
    public float waitTimer;

    public float dyingTime;
    public float dyingTimer;

    public float reelTime;
    public float reelTimer;

    public bool isChasing;
    public float sightRange;

    public int myMoveState;

    private Coroutine idleCoroutine;
    private Coroutine lungeCoroutine;

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

        facingDir = 1;

        lungeCDTimer = lungeCDTime;

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
        transform.localScale = new Vector2(facingDir, 1);

        pDist = Vector2.Distance(myPos, pPos);

        StateHandler();

        Timers();
    }

    void FixedUpdate()
    {

        myPos = rb.position;
        pPos = player.transform.position;

        grounded = GroundCheck();

        if (myMoveState == 2)
            Chase();

        Gravity();
        Drag();

        rb.velocity = new Vector2(xVel, yVel);

        Animations();
    }

    void OnGetHit(GameObject hitBy, float damage, Vector2 knockback)
    {
        reelTimer = reelTime;

        xVel = knockback.x;
        
        if (!grounded)
        {
            xVel = knockback.x * 2f;
            yVel = knockback.y;
        }
        
        SwitchStates((MoveState)2);
    }

    private void StartIdle()
    {
        if (lungeCoroutine != null)
        {
            StopCoroutine(Lunge());
            lungeCoroutine = null;
        }

        if (idleCoroutine == null)
        {
            idleCoroutine = StartCoroutine(Idle());
        }
    }

    private IEnumerator Idle()
    {
        while (true)
        {
            waitTimer = 0;
            walkTimer = 0;

            waitTimer = waitTime;

            while (waitTimer > 0)
            {
                if (reelTimer > 0)
                    yield return new WaitUntil(() => reelTimer == 0);
                
                yield return null;
            }

            walkTimer = walkTime;
            facingDir = Random.Range(0, 2) * 2 - 1;
                
            while (walkTimer > 0)
            {
                if (reelTimer > 0)
                    yield return new WaitUntil(() => reelTimer == 0);
                
                if (FrontCheck())
                {
                    facingDir *= -1;
                }
                    
                xVel = walkSpeed * facingDir;

                yield return null;
            }
        }
    }

    private void StartChase()
    {
        if (idleCoroutine != null)
        {
            StopCoroutine(idleCoroutine);
            idleCoroutine = null;
        }

        if (lungeCoroutine != null)
        {
            StopCoroutine(lungeCoroutine);
            lungeCoroutine = null;
            lungeCDTimer = lungeCDTime;
        }
    }

    private void Chase()
    {
        if (reelTimer > 0)
            return;

        facingDir = (int)Mathf.Sign(pPos.x - myPos.x);

        xVel = Mathf.Clamp(xVel + (chaseAccel * Time.fixedDeltaTime * facingDir), -maxChaseSpeed, maxChaseSpeed);
    }

    private void StartLunge()
    {
        if (idleCoroutine != null)
        {
            StopCoroutine(idleCoroutine);
            idleCoroutine = null;
        }

        if (lungeCoroutine == null)
            lungeCoroutine = StartCoroutine(Lunge());
    }

    private IEnumerator Lunge()
    {
        lungeStep = 0;
        if (reelTimer == 0)
            xVel = 0;

        yield return new WaitForSeconds(crouchTime);

        lungeStep = 1;

        xVel = lungeForce.x * facingDir;
        yVel = lungeForce.y;

        yield return new WaitUntil(() => grounded && yVel <= 0);

        lungeStep = 2;

        lungeCDTimer = lungeCDTime;
    }

    private void StartFall()
    {

    }

    private void StateHandler()
    {
        if (myMoveState == 0)
            if (grounded)
                SwitchStates((MoveState)1);
        
        if (myMoveState == 1)
        {
            if (!grounded)
                SwitchStates((MoveState)0);

            if (pDist < sightRange && !Physics2D.Raycast(myPos, (pPos - myPos), pDist, ground))
                SwitchStates((MoveState)2);
        };
        if (myMoveState == 2)
        {
            if (!grounded)
                SwitchStates((MoveState)0);

            if (pDist > sightRange)
                SwitchStates((MoveState)1);
            
            if (pDist < lungeRange && lungeCDTimer == 0)
                SwitchStates((MoveState)3);
        }
        if (myMoveState == 3)
        {
            if (grounded && yVel <= 0 && lungeStep == 2)
                SwitchStates((MoveState)2);
            
            if (reelTimer > 0 && lungeStep == 0)
                SwitchStates((MoveState)2);
        }
    }

    private void SwitchStates(MoveState state)
    {
        myMoveState = (int)state;

        switch (state)
        {
            case MoveState.None:
                StartFall();
                break;

            case MoveState.Idling:
                StartIdle();
                break;

            case MoveState.Chasing:
                StartChase();
                break;
            
            case MoveState.Striking:
                StartLunge();
                break;
        }
    }



    private void Walk()
    {
        if (FrontCheck())
            facingDir *= -1;
        xVel = walkSpeed * facingDir;
    }

    private bool FrontCheck()
    {
        return Physics2D.BoxCast(myPos, col.bounds.extents * 0.8f, 0, Vector2.right * facingDir, 0.2f, ground);
    }

    public bool GroundCheck()
    {
        float halfHeight = col.bounds.size.y / 2f;
        groundCheck = Physics2D.BoxCast(myPos - new Vector2(0, halfHeight), new Vector2(col.bounds.size.x, 0.05f), 0, Vector2.down, 0.1f, ground);
        return (groundCheck);
    }

    private void Gravity()
    {
        if (!grounded)
        {
            yVel -= gravity * Time.fixedDeltaTime;
        }
        else if (yVel < 0)
        {
            yVel = -2f;
        }
    }



    private void Drag()
    {
        if ((myMoveState == 1 && waitTimer > 0) || (myMoveState == -1 && grounded))
        {
            xVel *= Mathf.Exp(-groundDrag * Time.fixedDeltaTime);
        }
        if (myMoveState == 0 || (myMoveState == 4 && !grounded))
        {
            xVel *= Mathf.Exp(-airDrag * Time.fixedDeltaTime);
        }
    }

    private void OnDeath(GameObject hitBy, float damage, Vector2 knockback)
    {
        StartCoroutine(DeathProcess());
    }

    private IEnumerator DeathProcess()
    {
        dyingTimer = dyingTime;
        anim.SetBool("isDying", true);
        myMoveState = -1;
        StopCoroutine(Idle());

        yield return new WaitUntil(() => dyingTimer == 0);

        Destroy(gameObject);
    }

    private void Animations()
    {
        if (myMoveState == 1)
        {
            anim.SetBool("isWaiting", waitTimer > 0);
            anim.SetBool("isWalking", walkTimer > 0);
        }
        else
        {
            anim.SetBool("isWaiting", false);
            anim.SetBool("isWalking", false);
        }
        anim.SetBool("isChasing", myMoveState == 2);
        anim.SetBool("isHurt", reelTimer > 0);
        anim.SetBool("isSquating", myMoveState == 3 && lungeStep == 0);
        anim.SetBool("isLunging", lungeStep == 1);
    }

    void Timers()
    {
        if (walkTimer > 0)
            walkTimer -= Time.deltaTime;
        else
            walkTimer = 0;
        
        if (waitTimer > 0)
            waitTimer -= Time.deltaTime;
        else
            waitTimer = 0;
        
        if (reelTimer > 0)
            reelTimer -= Time.deltaTime;
        else
            reelTimer = 0;
        
        if (dyingTimer > 0)
            dyingTimer -= Time.deltaTime;
        else
            dyingTimer = 0;
        
        if (lungeCDTimer > 0)
            lungeCDTimer -= Time.deltaTime;
        else
            lungeCDTimer = 0;
    }
}