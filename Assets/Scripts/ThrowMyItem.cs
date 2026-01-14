using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowMyItem : MonoBehaviour
{    
    private Rigidbody2D rb;

    public Vector2 throwDir;

    public float flightVel;

    public Vector3 myPos;

    private Vector3 mouseWorldPos;

    public GameObject throwingRings;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }


    void Update()
    {
        myPos = rb.position;

        TakeInputs();
    }

    void TakeInputs()
    {
        mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(1))
        {
            throwDir = (mouseWorldPos - myPos).normalized;
                    
            GameObject thrown = Instantiate(throwingRings, myPos, Quaternion.identity);

            ThrowingRing thrownScript = thrown.GetComponent<ThrowingRing>();
            thrownScript.flyDir = throwDir;

        }
    }
}
