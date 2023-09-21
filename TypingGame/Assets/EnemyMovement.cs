using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float _speed; // TODO: Have this in WPM
    [SerializeField] private Tilemap _pathTiles;

    private float _sinceLastMove;
    private Vector3Int[] _lastMoveOptions;

    private Vector2Int _direction = Vector2Int.up;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        _sinceLastMove += Time.deltaTime;
    }

    private void Move()
    {
        var moveOptions = GetMoveOptions();

        _sinceLastMove = 0;
    }

    private Vector2Int[] GetMoveOptions()
    {
        var position = _pathTiles.WorldToCell(transform.position);
        return LevelTiles.Instance
            .GetNeighboursOf(position)
            .Select(t => (Vector2Int)t.Position)
            .ToArray();
    }
}
