using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyBody : MonoBehaviour
{
    public event Action<GameObject, float, Vector2> OnTakeDamage;
    public event Action OnDeath;

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
        if (currentHealth > 0)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                Die();
                return;
            }

            OnTakeDamage?.Invoke(hitBy, damage, knockback);
        }
    }

    public void Die()
    {
        OnDeath?.Invoke();
    }

    void Timers()
    {

    }

    void OnTriggerEnter2D(Collider2D hit)
    {
        
    }
}
