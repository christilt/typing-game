using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class tilemaptest : MonoBehaviour
{
    private Tilemap _tilemap;
    void Start()
    {
        _tilemap = GetComponent<Tilemap>();

        for (int i = 0; i < _tilemap.cellBounds.size.x; i++)
        {
            for (int j = 0; j < _tilemap.cellBounds.size.y; j++)
            {
                Vector3Int tilePosition = new Vector3Int(i, j, 0);
                TileBase tile = _tilemap.GetTile(tilePosition);
                if (tile != null)
                {
                    Tile tileData = (Tile)tile;
                    Debug.Log(tileData.color);
                }
            }
        }
    }
}
