using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thingy_Movement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D col;
    private Animator anim;
    private SpriteRenderer rend;

    private EnemyBody myBody;

    private GameObject player;
    private Vector2 pPos;
    private Vector2 pDir;
    
    private Vector2 myPos;

    public float speed;

    int facingDir;

    public float xVel;
    public float yVel;

    public float gravity;
    public float airDrag;

    public TimeStop timeStop;

    public Vector2 targetPos;

    public float reelTime;
    private float reelTimer;

    public float speedCap;

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

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        myBody = GetComponent<EnemyBody>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();

        player = GameObject.FindWithTag("Player");

        facingDir = 1;
    }

    // Update is called once per frame
    void Update()
    {
        myPos = transform.position;
        pPos = player.transform.position;

        pDir = (pPos - myPos).normalized;

        Animation();

        Timers();


    }

    void FixedUpdate()
    {
        //Gravity();
        if (reelTimer > 0)
            Drag();
        else
            Move();
        
        if (reelTimer > 0)
            WallCheck();

        rb.velocity = new Vector2(xVel, yVel);
    }

    void OnGetHit(DamageInfo info)
    {
        /*
        reelTimer = reelTime;

        timeStop.RequestFreeze(0.6f);

        Vector2 knockbackMult = new Vector2(20f, 1f);

        xVel = knockback.x * knockbackMult.x;
        yVel = knockback.y * knockbackMult.y;
        */
    }

    void WallCheck()
    {
        RaycastHit2D backCheck = Physics2D.Raycast(myPos, new Vector2(xVel, yVel), 0.6f, ground);

        if (backCheck)
        {
            timeStop.RequestFreeze(0.6f);

            Vector2 norm = backCheck.normal;

            xVel = Vector2.Reflect(rb.velocity, norm).x;
            yVel = Vector2.Reflect(rb.velocity, norm).y;
        }
    }

    void Move()
    {
        xVel = Mathf.Clamp(xVel + (speed * pDir.x * Time.fixedDeltaTime), -speedCap, speedCap);
        yVel = Mathf.Clamp(yVel + (speed * pDir.y * Time.fixedDeltaTime), -speedCap, speedCap);
    }

    void Gravity()
    {
        yVel -= gravity * Time.fixedDeltaTime;
    }

    void Drag()
    {
        xVel *= Mathf.Exp(-airDrag * Time.fixedDeltaTime);
    }

    void OnDeath(GameObject hitBy, float damage, Vector2 knockback)
    {

    }

    void Timers()
    {
        if (reelTimer > 0)
            reelTimer -= Time.deltaTime;
        else
            reelTimer = 0;
    }

    void Animation()
    {
        anim.SetBool("isHurt", reelTimer > 0);
    }
}
