using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public partial class LevelTiles : MonoBehaviour
{
    [SerializeField] private Tilemap _pathTiles;
    [SerializeField] private GameObject _keyIconPrefab;

    private IReadOnlyDictionary<Vector3Int, KeyTile> _keyTilesByPosition;


    private void Awake()
    {
        Instance = this;

        _keyTilesByPosition = InstantiateKeyTilesByPosition();
    }

    public static LevelTiles Instance { get; private set; }

    public IEnumerable<KeyTile> GetNeighboursOf(Vector3Int position)
    {
        if (_keyTilesByPosition.TryGetValue(position + Vector3Int.up, out KeyTile upTile))
            yield return upTile;
        if (_keyTilesByPosition.TryGetValue(position + Vector3Int.down, out KeyTile downTile))
            yield return downTile;
        if (_keyTilesByPosition.TryGetValue(position + Vector3Int.left, out KeyTile leftTile))
            yield return leftTile;
        if (_keyTilesByPosition.TryGetValue(position + Vector3Int.right, out KeyTile rightTile))
            yield return rightTile;
    }
        
    private Dictionary<Vector3Int, KeyTile> InstantiateKeyTilesByPosition()
    {
        var keyTilesByPosition = new Dictionary<Vector3Int, KeyTile>();

        // See https://forum.unity.com/threads/how-to-get-tile-position.1454926/
        foreach (var position in _pathTiles.cellBounds.allPositionsWithin)
        {
            if (!_pathTiles.HasTile(position))
                continue;

            var keyTile = KeyTile.Instantiate(_keyIconPrefab, position, this.gameObject);
            keyTilesByPosition.Add(position, keyTile);
        }
        return keyTilesByPosition;
    }
}