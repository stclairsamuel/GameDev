using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyBody : MonoBehaviour
{
    public event Action<GameObject, float, Vector2> OnTakeDamage;
    public event Action OnDeath;

    private Rigidbody2D rb;
    private SpriteRenderer rend;

    public Material myMat;
    public Material flashMat;

    public float maxHealth;
    public float currentHealth;

    public float flashTime;
    public float flashTimer;

    public ParticleSystem hurtParticles;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (flashTimer > 0)
            rend.material = flashMat;
        else
            rend.material = myMat;

        Timers();
    }

    public void GetHit(GameObject hitBy, float damage, Vector2 knockback)
    {

        if (currentHealth > 0)
        {
            currentHealth -= damage;
            flashTimer = flashTime;

            SummonHurtParticles(Mathf.Sign(knockback.x) * Vector2.right);

            if (currentHealth <= 0)
            {
                Die();
                return;
            }

            OnTakeDamage?.Invoke(hitBy, damage, knockback);
        }
    }

    private void SummonHurtParticles(Vector2 attackDirection)
    {
        Quaternion rotation = Quaternion.FromToRotation(Vector2.right, attackDirection);

        Instantiate(hurtParticles, transform.position, rotation);
    }

    public void Die()
    {
        OnDeath?.Invoke();
    }

    void Timers()
    {
        if (flashTimer > 0)
            flashTimer -= Time.deltaTime;
        else
            flashTimer = 0;
    }

    void OnTriggerEnter2D(Collider2D hit)
    {
        
    }
}

public enum MoveState
{
    Dying = -1,
    None = 0,
    Idling = 1,
    Chasing = 2,
    Striking = 3
}
