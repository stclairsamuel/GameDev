using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingRing : MonoBehaviour
{
    private Rigidbody2D rb;

    public GameObject fallingItem;

    public float flightVel;

    public Vector2 flyDir;

    float bounceCount = 0;
    public float bounceCDTime = 0.05f;
    float bounceCDTimer;

    public float maxBounces;

    public float flyTime;
    float flyTimer;

    bool isFalling;

    public float gravity;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void Start()
    {
        rb.velocity = flyDir.normalized * flightVel;

        flyTimer = flyTime;
    }

    void Update()
    {
        Timers();
    }

    void FixedUpdate()
    {
        isFalling = flyTimer == 0;

        if (isFalling)
            Gravity();
    }

    void DestroySelf()
    {

    }

    void Gravity()
    {
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - (gravity * Time.fixedDeltaTime));
    }
    
    void OnCollisionEnter2D()
    {
        if (bounceCDTimer == 0)
        {

            bounceCDTimer = bounceCDTime;

            if (bounceCount >= maxBounces)
            {
                Destroy(gameObject);
            }

            bounceCount += 1f;
        }
    }

    void Timers()
    {
        if (bounceCDTimer > 0)
            bounceCDTimer -= Time.deltaTime;
        else
            bounceCDTimer = 0;

        if (flyTimer > 0)
            flyTimer -= Time.deltaTime;
        else
            flyTimer = 0;
    }
}