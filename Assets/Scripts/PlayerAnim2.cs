using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim2 : MonoBehaviour
{
    private Animator anim;
    private PlayerTracker myTracker;
    private PlayerMovement2 myMov;

    void OnEnable()
    {
        myTracker.OnGroundTouch += GroundTouch;
        myTracker.OnGroundLeave += GroundLeave;
        myTracker.OnWallTouch += WallTouch;
        myTracker.OnWallLeave += WallLeave;

        myTracker.Jump += Jump;
        
    }
    void OnDisable()
    {
        myTracker.OnGroundTouch -= GroundTouch;
        myTracker.OnGroundLeave -= GroundLeave;
        myTracker.OnWallTouch -= WallTouch;
        myTracker.OnWallLeave -= WallLeave;

        myTracker.Jump -= Jump;
    }

    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();
        myTracker = GetComponent<PlayerTracker>();
        myMov = GetComponent<PlayerMovement2>();
    }

    // Update is called once per frame
    void Update()
    {
        Animations();

        transform.localScale = new Vector3((!myTracker.grounded && myTracker.touchingWall ? -myTracker.facingDir : myTracker.facingDir), 1, 0);
    }

    void Animations()
    {
        anim.SetBool("isRunning", myTracker.xInput != 0);
    }

    void GroundTouch()
    {
        anim.SetBool("grounded", true);
    }
    void GroundLeave()
    {
        anim.SetBool("grounded", false);
    }

    void WallTouch()
    {
        anim.SetBool("touchingWall", true);
    }
    void WallLeave()
    {
        anim.SetBool("touchingWall", false);
    }

    void Jump()
    {
        anim.SetTrigger("jump");
    }


}
