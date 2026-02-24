using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingRing : MonoBehaviour
{
    private Rigidbody2D rb;

    public GameObject fallingItem;

    public float flightVel;

    public Vector2 flyDir;

    public float bounceVelDampen;

    public float xVel;
    public float yVel;

    float bounceCount = 0;
    public float bounceCDTime = 0.05f;
    float bounceCDTimer;

    public float maxBounces;

    public float flyTime;
    float flyTimer;

    bool isFalling;

    public float gravity;

    Vector3 myPos;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void Start()
    {
        Vector2 f = (flyDir.normalized * flightVel);

        xVel = f.x;
        yVel = f.y;

        flyTimer = flyTime;
    }

    void Update()
    {
        Timers();

        myPos = transform.position;
    }

    void FixedUpdate()
    {
        isFalling = flyTimer == 0;

        if (isFalling)
            Gravity();

        rb.velocity = new Vector2(xVel, yVel);
    }

    void DestroySelf()
    {

    }

    void Gravity()
    {
        yVel -= (gravity * Time.fixedDeltaTime);
    }
    
    void OnCollisionEnter2D(Collision2D impact)
    {
        if (impact.contactCount == 0)
        {
            return;
        }

        Debug.Log(impact.contactCount);

        foreach (ContactPoint2D c in impact.contacts)
        {
            Debug.Log(c.normal);
        }

        Vector2 norm = impact.contacts[0].normal;

        Vector2 reflectedVel = Vector2.Reflect(new Vector2(xVel, yVel), norm);

        xVel = reflectedVel.x * bounceVelDampen;
        yVel = reflectedVel.y * bounceVelDampen;

        if (bounceCDTimer == 0)
        {
            bounceCDTimer = bounceCDTime;

            if (bounceCount >= maxBounces)
            {
                KillMyself();
            }

            bounceCount += 1f;
        }
    }

    void KillMyself()
    {
        GameObject.Instantiate(fallingItem, rb.position, Quaternion.identity);

        Destroy(gameObject);
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