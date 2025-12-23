using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShamblerAttack : MonoBehaviour
{
    private ShamblerMovement myMov;
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
        myMov = GetComponentInParent<ShamblerMovement>();
        myBody = GetComponentInParent<EnemyBody>();
        col = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerTracker>(out PlayerTracker pTracker))
        {
            pTracker.Damage(gameObject, 2f, new Vector2(myMov.facingDir * knockback.x, knockback.y), 0.1f);
        }
    }

    void OnDeath()
    {
        col.enabled = false;
    }
}
