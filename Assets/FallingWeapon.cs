using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingWeapon : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D col;

    public float xVel;
    public float yVel;

    public float size;
    
    float halfWidth;

    public Vector2 myPos;

    public LayerMask ground;

    public GameObject groundedPrefab;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();

        col.size = new Vector2(size, size);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        halfWidth = (size/2f);
        myPos = transform.position;

        CheckGround();
    }

    void CheckGround()
    {
        float skinWidth = 0.02f;

        bool grounded = Physics2D.BoxCast(myPos, new Vector2(size, size), 0, Vector2.down, skinWidth, ground);

        if (grounded)
        {
            KillMyself();
        }
    }

    void KillMyself()
    {
        float skinWidth = 0.02f;
        float spawnBodySize = 0.5f;

        RaycastHit2D groundHit = Physics2D.Raycast(rb.position, Vector2.down, size/2f + skinWidth);

        Vector2 groundPos = groundHit.point;

        Vector2 spawnPos = new Vector2(groundPos.x, groundPos.y);

        GameObject groundedSelf = GameObject.Instantiate(groundedPrefab, spawnPos, Quaternion.identity);

        Destroy(gameObject);
    }
}
