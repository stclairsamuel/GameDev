using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThrowMyItem : MonoBehaviour
{    
    public InputActionAsset InputActions;

    public InputAction throwAction;

    private Rigidbody2D rb;

    public Vector2 throwDir;

    public float flightVel;

    public Vector3 myPos;

    private Vector3 mouseWorldPos;

    public GameObject throwingRings;

    void OnEnable()
    {
        
    }
    void OnDisable()
    {
        
    }

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        throwAction = InputActions.FindAction("Throw");
    }


    void Update()
    {
        myPos = rb.position;

        TakeInputs();
    }

    void TakeInputs()
    {
        mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public void ThrowItem()
    {
        throwDir = (mouseWorldPos - myPos).normalized;
                    
        GameObject thrown = Instantiate(throwingRings, myPos, Quaternion.identity);

        ThrowingRing thrownScript = thrown.GetComponent<ThrowingRing>();
        thrownScript.flyDir = throwDir;
    }
}
