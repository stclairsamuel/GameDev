using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public PlayerMovement pMov;
    public PlayerTracker pTracker;
    public PlayerAnimation pAnim;

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
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent<EnemyBody>(out EnemyBody hitBody))
        {
            hitBody.GetHit(gameObject, 0, 10f * pMov.facingDir);
        }
    }
}
