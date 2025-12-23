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

    public float xVel;
    public float yVel;

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
    
    public bool dead;

    public float reelTime;
    public float reelTimer;

    private bool isWalking;
    private bool isWaiting;

    public bool isChasing;
    public float sightRange;

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
        transform.localScale = new Vector2(facingDir, 1);

        Timers();
    }

    void FixedUpdate()
    {
        if (dead)
        {
            xVel = 0;
            if (dyingTimer == 0)
            {
                Destroy(gameObject);
            }
        }

        myPos = rb.position;
        pPos = player.transform.position;

        grounded = GroundCheck();

        Gravity();
        Drag();

        if (!dead)
        {
            if (!isChasing)
            {
                Idle();
            }
            if (isChasing)
            {
                Chase();
            }
        }

        rb.velocity = new Vector2(xVel, yVel);

        Animations();
    }

    void OnGetHit(GameObject hitBy, float damage, Vector2 knockback)
    {
        reelTimer = reelTime;

        if (grounded)
        {
            xVel = knockback.x;
        }
    }

    private void Idle()
    {

        float pDist = Vector2.Distance(myPos, pPos);

        isChasing = pDist < sightRange && !Physics2D.Raycast(myPos, (pPos - myPos), sightRange, ground);

        if (reelTimer > 0)
            return;

        bool wasWalking = isWalking;
        isWalking = walkTimer > 0;

        if (wasWalking && !isWalking)
        {
            waitTimer = waitTime;
        }

        if (!isWalking && waitTimer == 0)
        {
            StartWalking();
        }

        isWaiting = !isWalking;

        if (isWalking)
        {
            xVel = walkSpeed * facingDir;
        }
    }

    private void Chase()
    {
        float pDist = Vector2.Distance(myPos, pPos);
        if (pDist > sightRange)
        {
            isChasing = false;
            waitTimer = waitTime;
            return;
        }

        facingDir = (int)Mathf.Sign(pPos.x - myPos.x);

        xVel = Mathf.Clamp(xVel + (chaseAccel * Time.deltaTime * facingDir), -maxChaseSpeed, maxChaseSpeed);
    }

    private void StartWalking()
    {
        facingDir = Random.Range(0, 2) * 2 - 1;

        walkTimer = walkTime;
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
        if (!isChasing)
        {
            if (grounded)
            {
                xVel *= Mathf.Exp(-groundDrag * Time.fixedDeltaTime);
            }
            else
            {
                xVel *= Mathf.Exp(-airDrag * Time.fixedDeltaTime);
            }
        }
    }

    private void OnDeath()
    {
        dyingTimer = dyingTime;
        dead = true;
        anim.SetBool("isDying", true);
        xVel = 0;
    }

    private void Animations()
    {
        anim.SetBool("isWaiting", isWaiting);
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isChasing", isChasing);
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
    }
}
