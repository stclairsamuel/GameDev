using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerAttack : MonoBehaviour
{
    private SpriteRenderer rend;
    private PlayerTracker myTracker;

    public List<GameObject> attacks;

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

    public int attackStep = 0;

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
        hitObjects = new List<Collider2D>();
        resetTimer = resetTime + attackCDTime;
        attackCDTimer = attackCDTime;

        GameObject newSlice = Instantiate(attacks[attackStep]);
        PlayerSliceAnim sliceScript = newSlice.GetComponent<PlayerSliceAnim>();
        sliceScript.attackController = gameObject.GetComponent<PlayerAttack>();

        if (attackStep < attacks.Count - 1)
            attackStep += 1;
        else
            attackStep = 0;
    }

    public void SuccessfulHit()
    {
        successfulHit?.Invoke();
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

}
