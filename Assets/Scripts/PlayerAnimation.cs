using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator anim;

    public PlayerTracker pTracker;
    public PlayerMovement pMov;

    public bool isFacingRight;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("isRunning", pMov.xInput != 0);
        
        anim.SetBool("isClimbing", pTracker.grabbingWall);

        anim.SetBool("isFalling", !pTracker.grounded && !pTracker.grabbingWall);

        isFacingRight = pMov.facingDir == 1;

        if (isFacingRight)
        {
            if (pTracker.grabbingWall)
            {
                transform.localScale = new Vector2(pTracker.myScale.x, transform.localScale.y);
            }
            else
            {
                transform.localScale = new Vector2(-pTracker.myScale.x, transform.localScale.y);
            }
        }
        else
        {
            if (pTracker.grabbingWall)
            {
                transform.localScale = new Vector2(-pTracker.myScale.x, transform.localScale.y);
            }
            else
            {
                transform.localScale = new Vector2(pTracker.myScale.x, transform.localScale.y);
            }
        }
    }
}
