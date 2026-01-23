using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash2 : MonoBehaviour
{
    private PlayerTracker myTracker;

    public float dashForce;

    private int facingDir;



    // Start is called before the first frame update
    void Awake()
    {
        myTracker = GetComponent<PlayerTracker>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartDash()
    {

    }
}
