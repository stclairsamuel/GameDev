using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyBody : MonoBehaviour
{
    public event Action<GameObject, float, Vector2> OnTakeDamage;

    private Rigidbody2D rb;

    public float maxHealth;
    public float currentHealth;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        Timers();
    }

    public void GetHit(GameObject hitBy, float damage, Vector2 knockback)
    {
        currentHealth -= damage;

        OnTakeDamage?.Invoke(hitBy, damage, knockback);
    }

    void Timers()
    {

    }

    void OnTriggerEnter2D(Collider2D hit)
    {
        
    }
}
