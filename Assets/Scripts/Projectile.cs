using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private PlayerTracker player;
    private TimeStop timeStop;

    public float xVel;
    public float yVel;

    public float gravity;

    public Vector2 myPos;

    public float throwForce;
    public float slowMult;
    public Vector2 throwDir;

    public bool isThrown;

    public float throwTime;
    public float throwTimer;

    public Rigidbody2D rb;

    public LayerMask ground;

    // Start is called before the first frame update
    void Start()
    {
        player = Object.FindFirstObjectByType<PlayerTracker>();
    }

    // Update is called once per frame
    void Update()
    {
        myPos = transform.position;

        if (isThrown && throwTimer > 0)
        {
            Fly();
        }
        if (isThrown && throwTimer == 0)
        {
            Fall();
        }
        
        Timers();

        RaycastHit2D wallCheck = Physics2D.Raycast(myPos, rb.velocity.normalized, 0.25f,ground);
        Debug.DrawRay(transform.position, rb.velocity.normalized, Color.green);

        if (wallCheck)
        {
            Destroy(gameObject);
        }

        rb.velocity = new Vector2(xVel, yVel);
    }

    public void GetThrown(Vector2 startPos, Vector2 targetPos)
    {
        transform.position = startPos;
        throwDir = (targetPos - startPos).normalized;
        isThrown = true;
        throwTimer = throwTime;
    }

    public void Fly()
    {
        if (throwTimer > 0)
        {
            xVel = throwDir.x * throwForce;
            yVel = (throwDir.y) * throwForce;
        }
    }

    public void Fall()
    {
        xVel += -Mathf.Sign(xVel) * slowMult * Time.deltaTime;
        yVel += -gravity * Time.deltaTime;
    }

    void Timers()
    {
        if (throwTimer > 0)
        {
            throwTimer -= Time.deltaTime;
        }
        if (throwTimer < 0)
        {
            throwTimer = 0;
        }
    }

    void OnTriggerEnter2D(Collider2D contact)
    {
        if (contact.CompareTag("Player"))
        {
            player.Damage(gameObject, 1f, new Vector2(Mathf.Sign(xVel) * 5f, 5f));

            Destroy(gameObject);
        }

        if (contact.gameObject.layer == ground)
        {
            Destroy(gameObject);
        }
    }
}
