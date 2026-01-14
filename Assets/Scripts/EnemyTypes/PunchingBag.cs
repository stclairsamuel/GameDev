using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchingBag : MonoBehaviour
{
    private EnemyBody myBody;
    
    private Rigidbody2D rb;
    private Collider2D col;

    public float gravity;
    private RaycastHit2D groundCheck;
    public float airDrag;
    public float groundDrag;

    public Vector2 myPos;

    public float knockbackMult;

    public LayerMask ground;
    public bool grounded;

    public float xVel;
    public float yVel;

    private void OnEnable()
    {
        myBody.OnTakeDamage += OnGetHit;
    }
    private void OnDisable()
    {
        myBody.OnTakeDamage -= OnGetHit;
    }

    private void Awake()
    {
        myBody = GetComponent<EnemyBody>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        myPos = rb.position;
    }

    void FixedUpdate()
    {
        grounded = GroundCheck();

        Gravity();
        Drag();
        WallCheck();

        rb.velocity = new Vector2(xVel, yVel);
    }

    private void OnGetHit(DamageInfo info)
    {
        xVel = info.Knockback.x * knockbackMult;
        yVel = info.Knockback.y * knockbackMult;
    }

    bool GroundCheck()
    {
        float halfHeight = col.bounds.size.y / 2f;
        groundCheck = Physics2D.BoxCast(myPos - new Vector2(0, halfHeight), new Vector2(col.bounds.size.x, 0.05f), 0, Vector2.down, 0.1f, ground);
        return (groundCheck);
    }

    void WallCheck()
    {
        float halfHeight = col.bounds.size.y / 2f;

        bool wallLeft = Physics2D.BoxCast(myPos, col.bounds.size * 0.9f, 0, Vector2.left, 0.02f, ground);
        bool wallRight = Physics2D.BoxCast(myPos, col.bounds.size * 0.9f, 0, Vector2.right, 0.02f, ground);
        bool wallTop = Physics2D.BoxCast(myPos + new Vector2(0, halfHeight), new Vector2(col.bounds.size.x, 0.05f), 0, Vector2.up, 0.1f, ground);

        if (wallLeft)
        {
            xVel = Mathf.Clamp(xVel, 0, Mathf.Infinity);
        }
        if (wallRight)
        {
            xVel = Mathf.Clamp(xVel, -Mathf.Infinity, 0);
        }

        if (wallTop)
        {
            yVel = Mathf.Clamp(yVel, -Mathf.Infinity, 0);
        }
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
}
