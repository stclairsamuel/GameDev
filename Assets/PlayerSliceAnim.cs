using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSliceAnim : MonoBehaviour
{
    private Animator anim;

    private GameObject player;

    private int myFacingDir;

    private List<Collider2D> hitObjects;
    public PlayerAttack attackController;
    private PlayerTracker myTracker;

    public float sliceTime;

    void Awake()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");

        myTracker = player.GetComponent<PlayerTracker>();

        hitObjects = new List<Collider2D>();

        transform.position = player.transform.position + new Vector3(0.5f * myFacingDir, 0.1f, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        myFacingDir = myTracker.facingDir;
        transform.localScale = new Vector2(myFacingDir, 1);

        

        Destroy(gameObject, sliceTime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position + new Vector3(0.5f * myFacingDir, 0.1f, 0);
    }

    void OnTriggerEnter2D(Collider2D objectHit)
    {
        if (objectHit.TryGetComponent<EnemyBody>(out EnemyBody target))
        {
            if (!hitObjects.Contains(objectHit))
            {
                Vector2 knockBackDir = new Vector2(myFacingDir, 1);
                DamageInfo info = new DamageInfo(gameObject, attackController.damage, knockBackDir, attackController.knockback);
                target.GetHit(info);
                hitObjects.Add(objectHit);
                attackController.SuccessfulHit();
            }
        }
    }   
}
