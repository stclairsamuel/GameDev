using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MawbatAttack : MonoBehaviour
{
    private MawbatMovement myMov;
    private EnemyBody myBody;
    public Vector2 knockback;
    private Collider2D col;

    void OnEnable()
    {
        myBody.OnDeath += OnDeath;
    }
    void OnDisable()
    {
        myBody.OnDeath -= OnDeath;
    }

    // Start is called before the first frame update
    void Awake()
    {
        myMov = GetComponentInParent<MawbatMovement>();
        myBody = GetComponentInParent<EnemyBody>();
        col = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerTracker>(out PlayerTracker pTracker))
        {
            DamageInfo info = new DamageInfo(gameObject, 1f, new Vector2(myMov.facingDir * knockback.x, knockback.y), 1f, 0.1f);

            pTracker.Damage(info);
        }
    }

    void OnDeath(GameObject hitBy, float damage, Vector2 knockback)
    {
        col.enabled = false;
    }
}
