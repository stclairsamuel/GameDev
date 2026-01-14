using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingRing : MonoBehaviour
{
    private Rigidbody2D rb;

    public float flightVel;

    public Vector2 flyDir;

    float bounceCount = 0;
    public float bounceCDTime = 0.05f;
    float bounceCDTimer;

    public float maxBounces;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void Start()
    {
        rb.velocity = flyDir.normalized * flightVel;
    }

    void Update()
    {
        if (bounceCDTimer > 0)
            bounceCDTimer -= Time.deltaTime;
        else
            bounceCDTimer = 0;
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
}