using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TilemapExtensions
{
    public static IReadOnlyCollection<Vector2Int> GetPositions(this Tilemap tilemap)
    {
        var positions = new List<Vector2Int>();

        // See https://forum.unity.com/threads/how-to-get-tile-position.1454926/
        foreach (var position in tilemap.cellBounds.allPositionsWithin)
        {
            if (!tilemap.HasTile(position))
                continue;

            positions.Add((Vector2Int)position);
        }
        return positions.AsReadOnly();
    }
}