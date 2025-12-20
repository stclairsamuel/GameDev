using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazards : MonoBehaviour
{
    public GameObject player;

    public PlayerTracker pTracker;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D touching)
    {
        if (touching == player.GetComponent<Collider2D>())
        {
            pTracker.Damage(gameObject, 1f, new Vector2(0, 20f));
        }
    }
}
