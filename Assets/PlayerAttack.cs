using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerAttack : MonoBehaviour
{
    private SpriteRenderer rend;
    private Animator anim;
    private PlayerTracker myTracker;
    private Collider2D col;

    public event Action successfulHit;

    public float attackCDTime;
    float attackCDTimer;
    public float attackBufferTime;
    float attackBufferTimer;
    public float resetTime;
    float resetTimer;

    public float knockback;
    public float damage;

    bool isAttacking = false;

    int attackStep = 0;

    private List<Collider2D> hitObjects;

    void OnEnable()
    {
        myTracker.Attack += AttackRecieved;
    }
    void OnDisable()
    {
        myTracker.Attack -= AttackRecieved;
    }

    // Start is called before the first frame update
    void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();

        myTracker = GetComponentInParent<PlayerTracker>();
    }

    // Update is called once per frame
    void Update()
    {
        if (attackBufferTimer > 0 && attackCDTimer == 0)
            StartAttack();
        
        isAttacking = attackCDTimer > 0;

        if (resetTimer == 0)
        {
            attackStep = 0;
        }

        if (attackCDTimer == 0)
        {
            col.enabled = false;
        }
            

        Timers();
    }

    void AttackRecieved()
    {
        if (isAttacking)
        {
            attackBufferTimer = attackBufferTime;
        }
        if (!isAttacking)
        {
            StartAttack();
        }

    }

    void StartAttack()
    {
        col.enabled = false;
        col.enabled = true;
        hitObjects = new List<Collider2D>();
        resetTimer = resetTime + attackCDTime;
        attackCDTimer = attackCDTime;
        if (attackStep < 3)
            attackStep += 1;
        else
            attackStep = 1;
        anim.SetInteger("attackStep", attackStep);
        anim.SetTrigger("Attack");
    }

    void Timers()
    {
        if (attackCDTimer > 0)
            attackCDTimer -= Time.deltaTime;
        else
            attackCDTimer = 0;
        
        if (attackBufferTimer > 0)
            attackBufferTimer -= Time.deltaTime;
        else
            attackBufferTimer = 0;
        
        if (resetTimer > 0)
            resetTimer -= Time.deltaTime;
        else
            resetTimer = 0;
    }

    void OnTriggerEnter2D(Collider2D objectHit)
    {
        if (objectHit.TryGetComponent<EnemyBody>(out EnemyBody target))
        {
            if (!hitObjects.Contains(objectHit))
            {
                Vector2 knockBackDir = new Vector2(myTracker.facingDir, 1);
                DamageInfo info = new DamageInfo(gameObject, damage, knockBackDir, knockback);
                target.GetHit(info);
                hitObjects.Add(objectHit);
                successfulHit?.Invoke();
            }
        }
    }
}
