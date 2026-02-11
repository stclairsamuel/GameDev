using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public float layer;
    public Transform player;

    public float yOffset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector2(player.position.x * (1f/layer), player.position.y * (1f/Mathf.Pow(layer, 2)) + yOffset);
    }
}
