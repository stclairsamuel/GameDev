using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyBody : MonoBehaviour
{
    public event Action<DamageInfo> OnTakeDamage;
    public event Action<GameObject, float, Vector2> OnDeath;

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

    public void GetHit(DamageInfo info)
    {

        if (currentHealth > 0)
        {
            currentHealth -= info.Damage;
            flashTimer = flashTime;

            SummonHurtParticles(Mathf.Sign(info.Knockback.x) * Vector2.right);

            if (currentHealth <= 0)
            {
                Die(info.HitBy, info.Damage, info.Knockback);
                return;
            }



            OnTakeDamage?.Invoke(info);
        }
    }

    private void SummonHurtParticles(Vector2 attackDirection)
    {
        Quaternion rotation = Quaternion.FromToRotation(Vector2.right, attackDirection);

        Instantiate(hurtParticles, transform.position, rotation);
    }

    public void Die(GameObject hitBy, float damage, Vector2 knockback)
    {
        OnDeath?.Invoke(hitBy, damage, knockback);
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

public struct DamageInfo
{
    public GameObject HitBy;
    public float Damage;
    public Vector2 Knockback;
    public float StunTime;

    public DamageInfo(
        GameObject hitBy,
        float damage,
        Vector2 knockbackDir,
        float knockbackMagnitude = 1,
        float stunTime = 0.2f
        )
    {
        HitBy = hitBy;
        Damage = damage;
        Knockback = knockbackDir * knockbackMagnitude;
        StunTime = stunTime;
    }
}
