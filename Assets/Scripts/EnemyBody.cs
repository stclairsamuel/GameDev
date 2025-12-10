using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBody : MonoBehaviour
{
    public float gravity;
    private Rigidbody2D rb;

    public float xVel;
    public float yVel;

    public Animator anim;

    public float hitTime;
    public float hitTimer;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //rb.velocity = new Vector2(xVel, yVel);

        anim.SetBool("isHit", (hitTimer > 0));

        Timers();
    }

    public void GetHit(GameObject hitBy, float damage, float knockback)
    {
        xVel = knockback * hitBy.transform.localScale.x;
        yVel = knockback;
        rb.velocity = new Vector2(knockback, Mathf.Abs(knockback));

        hitTimer = hitTime;

        transform.localScale = new Vector2(-2f * Mathf.Sign(hitBy.transform.position.x - transform.position.x), transform.localScale.y);
    }

    void Timers()
    {
        if (hitTimer > 0)
        {
            hitTimer -= Time.deltaTime;
        }
        else
        {
            hitTimer = 0;
        }
    }
}
