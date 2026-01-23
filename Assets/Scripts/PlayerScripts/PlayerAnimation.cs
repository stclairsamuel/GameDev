using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationLegacy : MonoBehaviour
{
    /*
    public Animator playerAnim;
    public Animator attackAnim;

    public PlayerTracker pTracker;
    public PlayerMovement pMov;

    public bool isFacingRight;

    public float attackTime;
    public float attackTimer;

    public Transform slash;
    public float slashOffset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerAnim.SetBool("isRunning", pMov.xInput != 0);
        
        playerAnim.SetBool("isClimbing", pTracker.grabbingWall);

        playerAnim.SetBool("isFalling", !pTracker.grounded && !pTracker.grabbingWall);

        playerAnim.SetBool("isAttacking", attackAnim.GetBool("isAttacking"));

        isFacingRight = pMov.facingDir == 1;

        if (!pTracker.grabbingWall)
            slash.position = new Vector2(pTracker.myPos.x + (slashOffset * pMov.facingDir), pTracker.myPos.y);
        else
            slash.position = new Vector2(pTracker.myPos.x - (slashOffset * pMov.facingDir), pTracker.myPos.y);

        if (Input.GetMouseButtonDown(0) && attackTimer == 0)
        {
            StartAttack();
        }
        if (attackTimer == 0 && attackAnim.GetBool("isAttacking"))
        {
            EndAttack();
        }

        if (isFacingRight)
        {
            transform.localScale = new Vector2(-pTracker.myScale.x, transform.localScale.y);
            if (pTracker.grabbingWall)
            {

                slash.localScale = new Vector2(-1f/pTracker.myScale.x, 1f/transform.localScale.y);
            }
            else
            {

                slash.localScale = new Vector2(1f/-pTracker.myScale.x, 1f/transform.localScale.y);
            }
        }
        else
        {
            transform.localScale = new Vector2(pTracker.myScale.x, transform.localScale.y);
            if (pTracker.grabbingWall)
            {

                slash.localScale = new Vector2(-1f/pTracker.myScale.x, 1f/transform.localScale.y);

            }
            else
            {
                slash.localScale = new Vector2(1f/-pTracker.myScale.x, 1f/transform.localScale.y);

            }
        }

        Timers();
    }

    void StartAttack()
    {
        attackAnim.SetBool("isAttacking", true);
        attackTimer = attackTime;
    }

    void EndAttack()
    {
        attackAnim.SetBool("isAttacking", false);
    }

    void Timers()
    {
        if (attackTimer > 0) { attackTimer -= Time.deltaTime; }
        else { attackTimer = 0; }
    }
    */
}
