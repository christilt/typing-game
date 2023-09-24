using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class LevelTiles : MonoBehaviour
{
    [SerializeField] private Tilemap _pathTiles;
    [SerializeField] private GameObject _keyIconPrefab;

    private IReadOnlyDictionary<Vector2Int, KeyTile> _keyTilesByPosition;


    private void Awake()
    {
        Instance = this;

        _keyTilesByPosition = InstantiateKeyTilesByPosition();
        Positions = _keyTilesByPosition.Keys.ToList();
    }

    public static LevelTiles Instance { get; private set; }

    public IReadOnlyCollection<Vector2Int> Positions { get; private set; }

    public IEnumerable<KeyTile> GetNeighboursOf(Vector3 position) => GetNeighboursOf((Vector2Int)_pathTiles.WorldToCell(position));

    public IEnumerable<KeyTile> GetNeighboursOf(Vector2Int position)
    {
        if (_keyTilesByPosition.TryGetValue(position + Vector2Int.up, out KeyTile upTile))
            yield return upTile;
        if (_keyTilesByPosition.TryGetValue(position + Vector2Int.down, out KeyTile downTile))
            yield return downTile;
        if (_keyTilesByPosition.TryGetValue(position + Vector2Int.left, out KeyTile leftTile))
            yield return leftTile;
        if (_keyTilesByPosition.TryGetValue(position + Vector2Int.right, out KeyTile rightTile))
            yield return rightTile;
    }

    // TODO avoid tiles like these: qaq
    private Dictionary<Vector2Int, KeyTile> InstantiateKeyTilesByPosition()
    {
        var positions = _pathTiles.GetPositions();
        return positions.
            ToDictionary(
                p => p, 
                p => KeyTile.Instantiate(_keyIconPrefab, (Vector3Int)p, this.gameObject));
    }
}