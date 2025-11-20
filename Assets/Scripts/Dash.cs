using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MonoBehaviour
{
    public float dashForce;
    public float dashTime;
    public float dashTimer;

    public bool isDashing;

    public float dashCooldownTime;
    public float dashCooldownTimer;

    public Rigidbody2D rb;

    public PlayerMovement pMov;

    public KeyCode dashKey = KeyCode.LeftShift;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(dashKey) && dashCooldownTimer == 0 && pMov.takeInput) 
        {
            DashStart();
        }

        if (isDashing && dashTimer == 0)
        {
            DashEnd();
        }

        Timers();
    }

    void DashStart()
    {
        isDashing = true;
        dashTimer = dashTime;
        dashCooldownTimer = dashCooldownTime + dashTime;
    }

    public void DashEnd()
    {
        isDashing = false;
        pMov.yVel = 0;

    }

    void Timers()
    {
        if (dashCooldownTimer > 0) { dashCooldownTimer -= Time.deltaTime; }
        else { dashCooldownTimer = 0; }
        if (dashTimer > 0) { dashTimer -= Time.deltaTime; }
        else { dashTimer = 0; }
    }
}
