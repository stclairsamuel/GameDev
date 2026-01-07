using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Hazards : MonoBehaviour
{
    public GameObject player;

    public PlayerTracker pTracker;

    private Tilemap hazardTilemap;

    // Start is called before the first frame update
    void Awake()
    {
        hazardTilemap = GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D touching)
    {

        if (touching == player.GetComponent<Collider2D>())
        {

            Vector2 hitPos = touching.bounds.center;
            Vector3Int cell = hazardTilemap.WorldToCell(hitPos);

            HazardTile tile = hazardTilemap.GetTile<HazardTile>(cell);

            if (tile != null)
                ApplyHazard(tile);
            
        }
    }

    void ApplyHazard(HazardTile tile)
    {
        pTracker.Damage(gameObject, tile.damage, tile.hitDirection * tile.knockback, 0, 0.2f);
    }
}
