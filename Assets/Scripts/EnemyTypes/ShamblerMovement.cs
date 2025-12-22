using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShamblerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D col;

    private EnemyBody myBody;

    public LayerMask isPlayer;
    private GameObject player;

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
    public float chaseSpeed;

    public float walkTime;
    public float walkTimer;
    public float waitTime;
    public float waitTimer;

    public float reelTime;
    public float reelTimer;

    private bool isWalking;
    private bool isWaiting;

    public bool isChasing;
    public float sightRange;

    void OnEnable()
    {
        myBody.OnTakeDamage += OnGetHit;
    }
    void OnDisable()
    {
        myBody.OnTakeDamage -= OnGetHit;
    }

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        myBody = GetComponent<EnemyBody>();
        col = GetComponent<Collider2D>();

        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        myPos = rb.position;

        Timers();
    }

    void FixedUpdate()
    {
        grounded = GroundCheck();

        Gravity();
        Drag();

        if (!isChasing)
        {
            Idle();
        }
        if (isChasing)
        {

        }

        rb.velocity = new Vector2(xVel, yVel);
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
        Vector2 pPos = player.transform.position;
        isChasing = Physics2D.Raycast(myPos, (pPos - myPos), sightRange, isPlayer);

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
        if (grounded)
        {
            xVel *= Mathf.Exp(-groundDrag * Time.fixedDeltaTime);
        }
        else
        {
            xVel *= Mathf.Exp(-airDrag * Time.fixedDeltaTime);
        }
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
    }
}
