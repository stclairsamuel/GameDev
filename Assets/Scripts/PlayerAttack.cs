using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerAttack : MonoBehaviour
{
    private SpriteRenderer rend;
    private PlayerTracker myTracker;

    public List<GameObject> attacks;

    public GameObject dashAttack;

    public event Action successfulHit;

    float attackCdTimer;
    public float attackBufferTime;
    public float attackBufferTimer;
    public float resetTime;
    float resetTimer;

    public float knockback;
    public float damage;

    public bool isAttacking = false;

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
        Timers();

        if (attackBufferTimer > 0 && attackCdTimer == 0)
            StartAttack();
        
        isAttacking = attackCdTimer > 0;

        if (resetTimer == 0)
        {
            attackStep = 0;
        }
    }

    void AttackRecieved()
    {
        StartAttack();
    }

    void StartAttack()
    {
        hitObjects = new List<Collider2D>();
        resetTimer = resetTime + myTracker.attackCdTime;

        bool isDashing = myTracker.isDashing;

        GameObject newSlice = isDashing ? Instantiate(dashAttack) : Instantiate(attacks[attackStep]);

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
        attackCdTimer = myTracker.attackCdTimer;
        
        if (resetTimer > 0)
            resetTimer -= Time.deltaTime;
        else
            resetTimer = 0;
    }

}
