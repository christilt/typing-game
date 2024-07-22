using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapTinter : MonoBehaviour
{
    private void Start()
    {
        var tilemaps = GetComponentsInChildren<Tilemap>();
        foreach(var tilemap in tilemaps)
        {
            Tint(tilemap, LevelSettingsManager.Instance.LevelSettings.WallColor);
        }
    }

    private void Tint(Tilemap tilemap, Color color)
    {
        for (int i = tilemap.cellBounds.x; i < tilemap.cellBounds.size.x; i++)
        {
            for (int j = tilemap.cellBounds.y; j < tilemap.cellBounds.size.y; j++)
            {
                for (int k = tilemap.cellBounds.z; k < tilemap.cellBounds.size.z; k++)
                {
                    var tilePosition = new Vector3Int(i, j, k);
                    var tile = tilemap.GetTile(tilePosition);
                    if (tile != null)
                    {
                        tilemap.RemoveTileFlags(tilePosition, TileFlags.LockColor);
                        tilemap.SetColor(tilePosition, color);
                        tilemap.SetTileFlags(tilePosition, TileFlags.LockColor);
                    }
                }
            }
        }
    }
}
