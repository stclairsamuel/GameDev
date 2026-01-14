using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public PlayerMovement pMov;
    public PlayerTracker pTracker;
    public PlayerAnimation pAnim;

    public Vector2 knockback;

    public List<Collider2D> hitObjects = new List<Collider2D>();

    private Collider2D hitBox;

    // Start is called before the first frame update
    void Start()
    {
        hitBox = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        hitBox.enabled = pAnim.attackTimer > 0;

        if (!hitBox.enabled && hitObjects.Count > 0)
        {
            hitObjects = new List<Collider2D>();
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (hitObjects.Contains(collider))
        {
            return;
        }

        if (collider.TryGetComponent<EnemyBody>(out EnemyBody hitBody))
        {
            DamageInfo info = new DamageInfo(
                gameObject,
                10f,
                new Vector2(knockback.x * pMov.facingDir, knockback.y)
            );

            hitBody.GetHit(info);
        }

        hitObjects.Add(collider);
    }
}
