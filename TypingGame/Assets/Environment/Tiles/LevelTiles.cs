using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using UnityEngine.UIElements;

public class LevelTiles : Singleton<LevelTiles>
{
    [SerializeField] private Tilemap _pathTiles;
    [SerializeField] private GameObject _keyIconPrefab;

    private IReadOnlyDictionary<Vector2Int, KeyTile> _keyTilesByPosition;

    public Tilemap PathTiles => _pathTiles;
    protected override void Awake()
    {
        base.Awake();

        _keyTilesByPosition = InstantiateKeyTilesByPosition();
        Positions = _keyTilesByPosition.Keys.ToList();
    }

    private void Start()
    {
        GameManager.Instance.OnStateChanging += OnGameStateChanging;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanging -= OnGameStateChanging;
    }

    private void OnGameStateChanging(GameState state)
    {
        if (state.EndsPlayerControl())
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

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

    // Naive approach to ensuring uniqueness - could be more efficient
    private Dictionary<Vector2Int, KeyTile> InstantiateKeyTilesByPosition()
    {
        var positions = _pathTiles.GetPositions();
        var keyTilesByPosition = new Dictionary<Vector2Int, KeyTile>();
        foreach (var position in positions)
        {
            var uniqueKeyBounds = new BoundsInt(
                xMin: position.x - 2,
                yMin: position.y - 2,
                zMin: 0,
                sizeX: 5,
                sizeY: 5,
                sizeZ: 1);

            var keysThatWouldBeDuplicates = new List<char>();
            foreach(var uniqueKeyBoundsPosition in uniqueKeyBounds.allPositionsWithin)
            {
                if (keyTilesByPosition.TryGetValue((Vector2Int) uniqueKeyBoundsPosition, out KeyTile keyTile))
                    keysThatWouldBeDuplicates.Add(keyTile.Key);
            }

            keyTilesByPosition.Add(
                position,
                KeyTile.Instantiate(
                    _keyIconPrefab, 
                    (Vector3Int)position, 
                    this.gameObject, 
                    deniedKeys: keysThatWouldBeDuplicates));
        }
        return keyTilesByPosition;
    }
}