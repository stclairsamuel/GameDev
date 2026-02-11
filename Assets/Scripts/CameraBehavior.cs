using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    private GameObject player;
    private PlayerTracker pTracker;

    public float viewSize;

    public Camera cam;

    Vector3 pPos;

    public float driftSpeed;

    public Vector3 centeredPos;

    public Vector3 offset;

    // Start is called before the first frame update
    void Awake()
    {
        cam.orthographicSize = viewSize;

        player = GameObject.FindWithTag("Player");
        pTracker = player.GetComponent<PlayerTracker>();

        cam = Camera.main;
    }

    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, centeredPos + offset, driftSpeed * Time.deltaTime);
    }

    // Update is called once per frame
    void Update()
    {
        pPos = player.transform.position + new Vector3(0, -10);

        offset = new Vector3(0, 10f, 0);

        centeredPos = pPos + new Vector3(0, 1.5f, -10f);
    }
}
