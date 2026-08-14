using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerTracker : MonoBehaviour
{
    public InputActionAsset InputActions;

    public InputActionMap playerActions;
    public InputActionMap uiActions;

    private InputAction m_jumpAction;
    private InputAction m_attackAction;
    private InputAction m_moveAction;
    private InputAction m_dashAction;
    private InputAction m_throwAction;
    private InputAction m_pause;
    private InputAction m_continue;

    private Vector2 m_moveAmt;

    public event Action OnGroundTouch;
    public event Action OnGroundLeave;
    public event Action OnWallTouch;
    public event Action OnWallLeave;

    public event Action Attack;

    public event Action Jump;
    public event Action Dash;

    float globalBufferTime = 0.2f;

    public float maxHealth;
    public float currentHealth;

    public float maxStamina;
    public float currentStamina;
    public float staminaRecoveryRate;
    public float dashStamina;

    public float stunTimer;

    private Rigidbody2D rb;
    private Collider2D col;

    private PlayerMovement2 myMov;
    private PlayerDash2 myDash;
    private PlayerAttack myAttack;
    private ThrowMyItem throwScript;

    private TimeStop tS;

    public LayerMask ground;
    public bool grounded;

    public int facingDir;

    public bool lockSpeed;
    
    [Header("Controls")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey = KeyCode.LeftShift;

    public float xInput;
    public float yInput;

    [Header("Throw Stuffs")]
    public float throwTime;
    public float throwTimer;
    public bool holdingThrow;
    Vector2 throwDir;
    public bool isThrowing;

    public float throwAirDrag;

    public float throwStallTime;
    public float throwStallTimer = 0;

    public float throwStallIntensity;

    [Header("Attack Stuffs")]

    public Dictionary<string, bool> activeAttackStates = new Dictionary<string, bool> {
        { "attacking" , false },
        { "rushing", false },
        { "dashAttacking", false }
    };

    public float dashAtkDrag;

    public float attackCdTime;
    public float attackCdTimer;

    public float dashAtkTime;
    public float dashAtkTimer;

    public bool dashAtkBuffered = false;
    public float timeHoldingRush = 0;

    public bool isRushing = false;
    
    [Header("Jump Stuffs")]
    public bool isJumping;
    public float jumpTime;
    float jumpTimer;

    public float coyoteTime;
    public float coyoteTimer;

    public float dashJumpTime;
    float dashJumpTimer;
    public bool canDashJump;

    public float landingSpeedTime;
    public float landingSpeedTimer;

    public float dashAtkDragTime;
    public float dashAtkDragTimer = 0;

    [Header("WallStuffs")]
    public bool touchingWall;
    public int wallTouched;
    public int lastWallTouched;
    public bool canWallJump;

    public bool canDashWallJump;

    public float superWallJumpTime;
    public float superWallJumpTimer;

    public float wallImpactStunTime;
    public bool impactedWall = false;

    [Header("Dash Stuffs")]
    public bool isDashing;
    public float dashTime;
    float dashTimer;

    public Vector2 myPos;
    private Vector2 mySize;

    public float halfHeight;
    public float halfWidth;

    public float savedXVel;

    public float hitStop;

    public ParticleSystem wallImpactParticles;

    Dictionary<string, float> bufferTimers = new Dictionary<string, float> {
        { "jump", 0 },
        { "attack", 0 },
        { "dash", 0 }
    };

    void OnEnable()
    {
        myAttack.successfulHit += HitEnemy;

        InputActions.FindActionMap("Player").Enable();
        InputActions.FindActionMap("UI").Disable();
    }
    void OnDisable()
    {
        myAttack.successfulHit -= HitEnemy;

        InputActions.FindActionMap("Player").Disable();
    }

    // Start is called before the first frame update
    void Awake()
    {
        playerActions = InputActions.FindActionMap("Player");
        uiActions = InputActions.FindActionMap("UI");

        m_jumpAction = InputActions.FindAction("Jump");
        m_attackAction = InputActions.FindAction("Attack");
        m_dashAction = InputActions.FindAction("Dash");
        m_moveAction = InputActions.FindAction("Move");
        m_throwAction = InputActions.FindAction("Throw");
        m_pause = InputActions.FindAction("Pause");
        m_continue = InputActions.FindAction("Continue");
 
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        myMov = GetComponent<PlayerMovement2>();
        myDash = GetComponent<PlayerDash2>();
        myAttack = GetComponentInChildren<PlayerAttack>();
        throwScript = GetComponent<ThrowMyItem>();

        tS = GameObject.FindWithTag("TimeStop").GetComponent<TimeStop>();

        

        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }

    void Start()
    {
        facingDir = 1;
    }
    
    void FixedUpdate()
    {
        if (xInput != 0)
        {
            facingDir = (int)xInput;
            myMov.Move();
        }
    }

    // Update is called once per frame
    void Update()
    {
        myPos = rb.position;
        mySize = col.bounds.size;

        halfHeight = mySize.y / 2f;
        halfWidth = mySize.x / 2f;

        lockSpeed = false;

        GroundCheck();
        WallsCheck();
        TopCheck();

        TakeInput();

        Timers();

        if (currentHealth <= 0)
        {
            SceneManager.LoadScene("DeathScreen");
        }

        if (currentStamina < maxStamina)
        {
            currentStamina += Time.deltaTime * staminaRecoveryRate;
        }
        else
        {
            currentStamina = maxStamina;
        }
    }

    void GroundCheck()
    {
        float skinWidth = 0.02f;

        float boxBottom = myPos.y - halfHeight;
        float boxLeft = myPos.x - halfWidth;

        Vector2 rayOrigin = new Vector2(boxLeft + skinWidth, boxBottom - skinWidth);
        float rayLength = mySize.x - (2f * skinWidth);

        bool wasGrounded = grounded;

        grounded = Physics2D.Raycast(rayOrigin, Vector2.right, rayLength, ground);

        if (wasGrounded && !grounded)
        {
            GroundLeave();
            OnGroundLeave?.Invoke();
        }
        if (!wasGrounded && grounded)
        {
            GroundTouch();
            OnGroundTouch?.Invoke();
        }
    }

    void WallsCheck()
    {
        //Variables

        float skinWidth = 0.02f;

        float boxBottom = myPos.y - halfHeight;
        float boxLeft = myPos.x - halfWidth;
        float boxRight = myPos.x + halfWidth;

        Vector2 leftOrigin = new Vector2(boxLeft - skinWidth, boxBottom + skinWidth);
        Vector2 rightOrigin = new Vector2(boxRight + skinWidth, boxBottom + skinWidth);

        float rayLength = mySize.y - (2f * skinWidth);

        bool wasTouchingWall = touchingWall;

        bool touchingLeft = Physics2D.Raycast(leftOrigin, Vector2.up, rayLength, ground);
        bool touchingRight = Physics2D.Raycast(rightOrigin, Vector2.up, rayLength, ground);

        touchingWall = touchingLeft || touchingRight;

        //Set Wall Touched

        if (touchingWall != wasTouchingWall)
        {
            if (touchingWall)
            {
                WallTouch();
                OnWallTouch?.Invoke();
            }
            else
            {
                WallLeave();
                OnWallLeave?.Invoke();
            }
        }

        if (touchingWall)
        {
            wallTouched = (touchingLeft ? -1 : 1);
            canWallJump = true;
        }
        
        if (!touchingWall)
            wallTouched = 0;
        
        if (wallTouched != 0)
            lastWallTouched = wallTouched;
        
        if (!touchingWall && coyoteTimer == 0)
        {
            canWallJump = false;
        }

        if (touchingLeft)
            myMov.xVel = Mathf.Clamp(myMov.xVel, 0, Mathf.Infinity);
        if (touchingRight)
            myMov.xVel = Mathf.Clamp(myMov.xVel, -Mathf.Infinity, 0);
    }

    void TopCheck()
    {
        float skinWidth = 0.02f;

        float boxTop = myPos.y + halfHeight;
        float boxLeft = myPos.x - halfWidth;

        Vector2 rayOrigin = new Vector2(boxLeft + skinWidth, boxTop + skinWidth);
        float rayLength = mySize.x - (2f * skinWidth);

        bool topCheck = Physics2D.Raycast(rayOrigin, Vector2.right, rayLength, ground);

        if (topCheck)
        {
            myMov.yVel = Mathf.Clamp(myMov.yVel, -Mathf.Infinity, 0);
            if (isJumping)
                StopJump();
        }
    }

    void TakeInput()
    {
        if (!isRushing)
            m_moveAmt = m_moveAction.ReadValue<Vector2>();
        else
            m_moveAmt = new Vector2(facingDir, 0);
        if (m_moveAmt.x != 0) m_moveAmt.x = Mathf.Sign(m_moveAmt.x);

        canDashJump = dashJumpTimer > 0;

        if (!(stunTimer > 0) && impactedWall)
        {
            impactedWall = false;
        }

        bool isStunned = stunTimer > 0;

        if (!isStunned)
        {
            xInput = m_moveAmt.x;
        }
        else
        {
            xInput = 0;
        }

        isDashing = dashTimer > 0;

        bool canJump = (grounded || touchingWall || coyoteTimer > 0) && !isStunned;
        bool canAttack = (attackCdTimer == 0 && !isDashing);

        if (m_jumpAction.WasPerformedThisFrame())
        {
            if (canJump)
                StartJump();
            else
                BufferInput("jump");
        }
        if (CheckBuffer("jump") && canJump)
        {
            StartJump();
            ReleaseBuffer("jump");
        }

        if ((m_jumpAction.WasReleasedThisFrame()  || (jumpTimer == 0)) && isJumping)
        {
            if (!isStunned)
                StopJump();
        }
        if (m_jumpAction.WasReleasedThisFrame())
        {
            ReleaseBuffer("jump");
        }

        if (isStunned)
            return;

        //dash input check
        if (m_dashAction.WasPerformedThisFrame() && currentStamina > dashStamina)
        {
            ExecuteDash();
        }

        //attack input check
        if (CheckBuffer("attack") && canAttack)
            ExecuteAttack();
            ReleaseBuffer("attack");

        if (m_attackAction.WasPerformedThisFrame())
        {
            if (canAttack)
                ExecuteAttack();
            else
                BufferInput("attack");
                if (dashAtkTimer > 0)
                    dashAtkBuffered = true;
                    BufferInput("attack");
        }
        if (m_attackAction.WasReleasedThisFrame())
        {
            ReleaseBuffer("attack");
        }

        if (m_throwAction.WasPerformedThisFrame() && !isThrowing)
        {
            StartThrow();
        }

        if (m_throwAction.WasReleasedThisFrame() && holdingThrow)
        {
            InitThrow();
        }

        if (throwTimer == 0 && isThrowing)
        {
            throwScript.ThrowItem();
            StopThrow();
        }

        if (isRushing)
            RunRushChecks();

        if (xInput == -Mathf.Sign(myMov.xVel) && xInput != 0 && isDashing)
            dashTimer = 0;
    }

    void ExecuteDash()
    {
        dashTimer = dashTime;
        dashJumpTimer = dashJumpTime;
        currentStamina -= dashStamina;
        myMov.StartDash();

        if (m_attackAction.IsPressed())
            StartRush();
        else
            dashAtkTimer = dashAtkTime + dashTime;
    }

    void StartRush()
    {
        Debug.Log("Rush Started");

        timeHoldingRush = 0;
        isRushing = true;

        activeAttackStates["rushing"] = true;
    }

    void ReleaseRush()
    {
        Debug.Log("Rush Released");

        isRushing = false;
        timeHoldingRush = 0;
    }

    void ExecuteRushAttack()
    {
        ReleaseRush();

        Debug.Log("Rush Attack Executed");

        myAttack.RushAttack();
    }

    void RunRushChecks()
    {
        if (touchingWall)
        {
            float wallCheckDist = 0.2f;

            bool facingWall = Physics2D.Raycast(myPos, Vector2.right * facingDir, wallCheckDist, ground);

            if (facingWall)
                WallSlamWhileRushing();
        }

        if (m_attackAction.WasReleasedThisFrame())
        {
            ExecuteRushAttack();
        }
    }

    void WallSlamWhileRushing()
    {
        ReleaseRush();
    }

    void ExecuteAttack()
    {
        Attack?.Invoke();

        attackCdTimer = attackCdTime;

        if (dashAtkTimer > 0)
        {
            dashAtkDragTimer = dashAtkDragTime;
        }
    }

    void StartThrow()
    {
        holdingThrow = true;
        throwStallTimer = throwStallTime;

        m_moveAction.Disable();
        m_dashAction.Disable();
    }
    void InitThrow()
    {
        holdingThrow = false;
        throwTimer = throwTime;
        isThrowing = true;
    }
    public void StopThrow()
    {
        holdingThrow = false;
        throwTimer = 0;
        isThrowing = false;
        m_moveAction.Enable();
        m_dashAction.Enable();
    }

    void StartJump()
    {
        isJumping = true;
        myMov.StartJump();
        Jump?.Invoke();
        jumpTimer = jumpTime;
    }
    void StopJump()
    {
        isJumping = false;
        myMov.StopJump();
        jumpTimer = 0;
    }

    void GroundTouch()
    {
        if (Mathf.Abs(myMov.xVel) > myMov.moveSpeed)
        {
            landingSpeedTimer = landingSpeedTime;
        }
        if (superWallJumpTimer > 0)
            superWallJumpTimer = 0;
    }
    void GroundLeave()
    {
        if (!isJumping)
            coyoteTimer = coyoteTime;
    }
    void WallTouch()
    {
        if (Mathf.Abs(myMov.xVel) > myMov.moveSpeed)
        {
            savedXVel = myMov.xVel;
            superWallJumpTimer = superWallJumpTime;
        }
        else
            superWallJumpTimer = 0;

        if (Mathf.Abs(myMov.xVel) > myMov.moveSpeed + 2f)
        {
            ImpactIntoWall();
        }
    }
    void WallLeave()
    {
        if (isJumping)
        {
            if (superWallJumpTimer > 0)
            {
                float arbitraryLeeway = 0.1f;
                superWallJumpTimer = arbitraryLeeway;
            }
        }

        else
            coyoteTimer = coyoteTime;

        impactedWall = false;
    }

    void ImpactIntoWall()
    {
        StunPlayer(wallImpactStunTime);

        if (myMov.yVel < 0)
            myMov.yVel = 0;

        impactedWall = true;

        SummonImpactParticles();


    }

    private void SummonImpactParticles()
    {
        Quaternion rotation = Quaternion.FromToRotation(Vector2.right, new Vector2(-facingDir, 0));

        Instantiate(wallImpactParticles, myPos + new Vector2(halfWidth * facingDir, 0), rotation);
    }



    public void Damage(DamageInfo info)
    {
        float freezeTime = 0.2f;

        tS.RequestFreeze(freezeTime);

        if (isThrowing || holdingThrow)
        {
            StopThrow();
        }

        currentHealth -= info.Damage;
        myMov.xVel = info.Knockback.x;
        myMov.yVel = info.Knockback.y;

        StunPlayer(info.StunTime);
    }

    public void StunPlayer(float stunTime)
    {
        stunTimer = stunTime;
    }

    public Vector2 GetDrag()
    {
        /* rules:
            player dashing? no drag
            play in air? less drag
            player on ground? lots of drag
            player holding throw? lots of drag which decreases over time
            player in dash strike? some drag
            take whichever drag is greatest
        */

        float dragToUse = 0;
        Vector2 dragAxis = Vector2.right;

        bool useThrowDrag = isThrowing || holdingThrow;

        bool useDashAtkDrag = dashAtkDragTimer > 0;

        bool keepLandingSpeed = landingSpeedTimer > 0;

        if (useThrowDrag)
        {
            float throwDragPercent = throwStallTimer / throwStallTime;

            float dragAmt = (1f - throwDragPercent) * throwStallIntensity;

            dragToUse = dragAmt;
            dragAxis = new Vector2(1, 1);

            if (throwDragPercent <= 0)
                StopThrow();
        }

        else if (useDashAtkDrag)
        {
            float dashAtkDragPercent = dashAtkDragTimer / dashAtkDragTime;

            dragToUse = (1f - dashAtkDragPercent) * dashAtkDrag;
            dragAxis = new Vector2(1, 1);
        }

        else if (isRushing)
        {
            dragToUse = 0;
        }

        else if (!isDashing && !keepLandingSpeed)
        {
            if (grounded)
            {
                if (xInput == 0)
                    dragToUse = myMov.groundIdleDrag;
                else if (Mathf.Abs(myMov.xVel) > myMov.moveSpeed)
                    dragToUse = myMov.groundDrag;
            }
            else
            {
                if (xInput == 0)
                    dragToUse = myMov.airIdleDrag;
                else if (Mathf.Abs(myMov.xVel) > myMov.moveSpeed)
                    dragToUse = myMov.airDrag;
            }
        }

        float accelMod = myMov.accelMod;

        float acceleration = myMov.normalAccel + accelMod;

        if (acceleration != myMov.normalAccel && !isDashing)
        {
            if (grounded)
                myMov.accelMod *= Mathf.Exp(-5f * Time.fixedDeltaTime);
            else
                myMov.accelMod *= Mathf.Exp(-2f * Time.fixedDeltaTime);
        }
        if (Mathf.Abs(accelMod) < 0.2f)
            myMov.accelMod = 0;

        return dragToUse * dragAxis;
    }

    public void HitEnemy()
    {
        if (Mathf.Abs(myMov.xVel) > myMov.moveSpeed)
        {
            myMov.xVel = facingDir * -6f;
            myMov.accelMod = -160f;

            if (!grounded)
            {
                float boost = 12f;
                myMov.yVel = boost;
            }
        }
        else
        {
            myMov.xVel = facingDir * -8f;
        }

        dashAtkDragTimer = 0;

        tS.RequestFreeze(hitStop);

        //landingSpeedTimer = landingSpeedTime;
    }

    void BufferInput(string inputToBuffer, float timeToBuffer = 0.2f)
    {
        try
        {
            bufferTimers[inputToBuffer] = timeToBuffer;
        }
        catch
        {
            Debug.Log(new string(inputToBuffer + " is not a bufferable input"));
        }
    }

    bool CheckBuffer(string bufferToCheck)
    {
        try
        {
            bool returnValue = bufferTimers[bufferToCheck] > 0;
            return returnValue;
        }
        catch
        {
            Debug.Log(new string(bufferToCheck + " is not a checkable input"));
            return false;
        }
    }

    void ReleaseBuffer(string bufferToRelease)
    {
        try
        {
            bufferTimers[bufferToRelease] = 0;
        }
        catch
        {
            Debug.Log(new string(bufferToRelease + " is not a setable input"));
        }
    }

    void Timers()
    {
        if (jumpTimer > 0)
            jumpTimer -= Time.deltaTime;
        else
            jumpTimer = 0;
        
        if (dashTimer > 0)
            dashTimer -= Time.deltaTime;
        else
            dashTimer = 0;
        
        if (dashJumpTimer > 0)
            dashJumpTimer -= Time.deltaTime;
        else
            dashJumpTimer = 0;

        if (coyoteTimer > 0)
            coyoteTimer -= Time.deltaTime;
        else
            coyoteTimer = 0;

        if (landingSpeedTimer > 0)
            landingSpeedTimer -= Time.deltaTime;
        else
            landingSpeedTimer = 0;

        if (superWallJumpTimer > 0)
            superWallJumpTimer -= Time.deltaTime;
        else
            superWallJumpTimer = 0;
        
        if (stunTimer > 0)
            stunTimer -= Time.deltaTime;
        else
            stunTimer = 0;
        
        if (throwTimer > 0)
            throwTimer -= Time.deltaTime;
        else
            throwTimer = 0;

        if (throwStallTimer > 0)
            throwStallTimer -= Time.deltaTime;
        else
            throwStallTimer = 0;

        if (dashAtkDragTimer > 0)
            dashAtkDragTimer -= Time.deltaTime;
        else
            dashAtkDragTimer = 0;

        if (attackCdTimer > 0)
            attackCdTimer -= Time.deltaTime;
        else
            attackCdTimer = 0;
        
        if (dashAtkTimer > 0)
            dashAtkTimer -= Time.deltaTime;
        else
            dashAtkTimer = 0;

        TickBufferTimers();
    }

    void TickBufferTimers()
    {
        List<string> keys = new List<string>(bufferTimers.Keys);

        foreach (string key in keys)
        {
            if (bufferTimers[key] > 0)
            {
                bufferTimers[key] -= Time.deltaTime;
                
                if (bufferTimers[key] < 0)
                {
                    bufferTimers[key] = 0;
                }
            }
        }
    }
}
