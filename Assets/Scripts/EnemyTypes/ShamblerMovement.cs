using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShamblerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D col;

    private EnemyBody myBody;

    public float gravity;

    public float xVel;
    public float yVel;

    public RaycastHit2D groundCheck;
    public LayerMask ground;
    public float groundDrag;
    public float airDrag;

    private Vector2 myPos;

    public float walkSpeed;
    public float chaseSpeed;

    public bool isChasing;

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
    }

    // Update is called once per frame
    void Update()
    {
        myPos = transform.position;

        Gravity();
        Drag();

        rb.velocity = new Vector2(xVel, yVel);

        Timers();
    }

    void OnGetHit(GameObject hitBy, float damage, Vector2 knockback)
    {
        if (Grounded())
        {
            xVel = knockback.x;
        }
    }

    bool Grounded()
    {
        float halfHeight = col.bounds.size.y / 2f;
        groundCheck = Physics2D.BoxCast(myPos - new Vector2(0, halfHeight), new Vector2(col.bounds.size.x, 0.05f), 0, Vector2.down, 0.1f, ground);
        return (groundCheck);
    }

    private void Gravity()
    {
        if (!Grounded())
        {
            yVel -= gravity * Time.deltaTime;
        }
        else if (yVel < 0)
        {
            yVel = -2f;
        }
    }

    private void Drag()
    {
        if (Grounded())
        {
            xVel *= Mathf.Exp(-groundDrag * Time.deltaTime);
        }
        else
        {
            xVel *= Mathf.Exp(-airDrag * Time.deltaTime);
        }
    }

    void Timers()
    {

    }
}
