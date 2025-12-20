using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RusherHitbox : MonoBehaviour
{
    public RusherBase rusher;

    public GameObject parent;

    private GameObject playerObject;

    private PlayerTracker player;

    public Vector2 hitboxOffset;

    Vector2 parentPos;

    public float knockback;
    public float freezeTime;
    public float stunTime;
    
    // Start is called before the first frame update
    void Start()
    {
        player = Object.FindFirstObjectByType<PlayerTracker>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        parentPos = parent.transform.position;

        if (rusher.chargeTimer > 0)
        {
            if (rusher.chargeTimer > rusher.slowTime)
            {
                hitboxOffset.x = 0.2f * Mathf.Sign(rusher.xVel);
            }
        }
        else
        {
            hitboxOffset.x = 0;
        }

        transform.position = parentPos + hitboxOffset;
    }

    void OnTriggerEnter2D(Collider2D hitBox)
    {
        if (hitBox.CompareTag("Player"))
        {
             player.Damage(parent, 2f, new Vector2(Mathf.Sign(playerObject.transform.position.x - parent.transform.position.x) * knockback, knockback), freezeTime, stunTime);
             if (rusher.chargeTimer > rusher.slowTime)
             {
                rusher.chargeTimer = rusher.slowTime;
             }
        }
    }
}
