using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class tilemaptest : MonoBehaviour
{
    [SerializeField] private Tilemap[] _tilemaps;
    [SerializeField] private Color _tint;

    private void Start()
    {
        foreach(var tilemap in _tilemaps)
        {
            Tint(tilemap);
        }
    }

    private void Tint(Tilemap tilemap)
    {
        for (int i = tilemap.cellBounds.x; i < tilemap.cellBounds.size.x; i++)
        {
            for (int j = tilemap.cellBounds.y; j < tilemap.cellBounds.size.y; j++)
            {
                for (int k = tilemap.cellBounds.z; k < tilemap.cellBounds.size.z; k++)
                {
                    Vector3Int tilePosition = new Vector3Int(i, j, k);
                    TileBase tile = tilemap.GetTile(tilePosition);
                    if (tile != null)
                    {
                        tilemap.RemoveTileFlags(tilePosition, TileFlags.LockColor);
                        tilemap.SetColor(tilePosition, _tint);
                        tilemap.SetTileFlags(tilePosition, TileFlags.LockColor);
                    }
                }
            }
        }
    }
}
