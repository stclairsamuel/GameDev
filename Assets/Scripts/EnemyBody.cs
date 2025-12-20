using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBody : MonoBehaviour
{
    private Rigidbody2D rb;

    public float xVel;
    public float yVel;

    public float hitTime;
    public float hitTimer;

    private RusherBase rushBase;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rushBase = TryGetComponent<RusherBase>(out rushBase) != null ? GetComponent<RusherBase>() : null;

        Debug.Log(rushBase);

    }

    // Update is called once per frame
    void Update()
    {
        Timers();
    }

    public void GetHit(GameObject hitBy, float damage, float knockback)
    {
        xVel = knockback * hitBy.transform.localScale.x;
        yVel = knockback;
        rb.velocity = new Vector2(knockback, Mathf.Abs(knockback));

        hitTimer = hitTime;
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

    void OnTriggerEnter2D(Collider2D hit)
    {
        
    }
}
