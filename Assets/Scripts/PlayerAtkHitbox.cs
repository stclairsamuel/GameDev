using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAtkHitbox : MonoBehaviour
{
    Rigidbody2D rb;

    public float knockbackMag;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponentInParent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<EnemyBody>(out EnemyBody target))
        {
            DamageInfo info = new DamageInfo(
                gameObject,
                2f,
                rb.velocity.normalized,
                knockbackMag,
                0.02f
            );
            target.GetHit(info);
        }
    }
}
