using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public Transform player;

    public float viewSize;

    public Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam.orthographicSize = viewSize;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.position + new Vector3(0, 0, -10);

        
    }
}
