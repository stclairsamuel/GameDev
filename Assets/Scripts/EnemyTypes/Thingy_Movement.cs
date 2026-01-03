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

    int facingDir;

    public float xVel;
    public float yVel;

    public float gravity;
    public float airDrag;

    public TimeStop timeStop;

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
        
    }

    void FixedUpdate()
    {
        Gravity();

        rb.velocity = new Vector2(xVel, yVel);
    }

    void OnGetHit(GameObject hitBy, float dmg, Vector2 knockback)
    {
        timeStop.RequestFreeze(0.6f);

        Vector2 knockbackMult = new Vector2(5f, 2.5f);

        xVel = knockback.x * knockbackMult.x;
        yVel = knockback.y * knockbackMult.y;
    }

    void Gravity()
    {
        yVel -= gravity * Time.fixedDeltaTime;
    }

    void Drag()
    {
        xVel *= Mathf.Exp(-airDrag);
    }

    void OnDeath()
    {

    }
}
