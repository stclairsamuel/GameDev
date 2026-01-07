using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(
    fileName = "New Hazard Tile",
    menuName = "Tiles/Hazard Tile"
)]
public class HazardTile : Tile
{
    [Header("Hazard Settings")]

    public int damage = 1;

    public Vector2 hitDirection = Vector2.up;

    public int knockback = 1;
}
