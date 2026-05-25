using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingRing : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D col;

    public GameObject fallingItem;

    public float flightVel;

    public Vector2 flyDir;

    public float bounceVelDampen;

    public float xVel;
    public float yVel;

    public float size;

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

        col = GetComponent<Collider2D>();
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
        Vector2 normToUse = Vector2.zero;

        if (impact.contactCount == 0)
        {
            return;
        }

        if (impact.contactCount == 1)
        {
            normToUse = impact.contacts[0].normal;
        }

        if (impact.contactCount > 1)
        {
            Vector2 avgVector = Vector2.zero;

            foreach (ContactPoint2D c in impact.contacts)
            {
                avgVector += c.normal;
            }

            avgVector /= impact.contactCount;

            normToUse = avgVector;
        }

        /*

        if (impact.contactCount == 2)
        {
            Vector2 origin = Vector2.Lerp(impact.contacts[0].point, impact.contacts[1].point, 0.5f);

            Vector2 velDir = new Vector2(xVel, yVel).normalized;

            Vector2 newPos = origin - (velDir);

            rb.position = newPos;
        }

        */

        Debug.Log(impact.contactCount);

        foreach (ContactPoint2D c in impact.contacts)
        {
            Debug.Log(c.normal);
        }

        Vector2 norm = impact.contacts[0].normal;

        Vector2 reflectedVel = Vector2.Reflect(new Vector2(xVel, yVel), norm);

        xVel = reflectedVel.x * bounceVelDampen;
        yVel = reflectedVel.y * bounceVelDampen;

        rb.velocity = new Vector2(xVel, yVel);

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