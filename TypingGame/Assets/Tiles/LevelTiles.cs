using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using TMPro;

public class LevelTiles : MonoBehaviour
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


    private class KeyTile
    {
        private readonly GameObject _icon;

        private KeyTile(GameObject icon, Vector3Int position, char key)
        {
            _icon = icon;
            Key = key;
            Position = position;
        }

        public char Key { get; }
        public Vector3Int Position { get; }

        public static KeyTile Instantiate(GameObject iconPrefab, Vector3Int position, GameObject parent)
        {
            var key = RandomKey();
            var icon = Object.Instantiate(original: iconPrefab, position: position, rotation: Quaternion.identity, parent: parent.transform);
            var iconText = icon.GetComponentInChildren<TextMeshProUGUI>();
            iconText.text = $"{key}";

            return new KeyTile(icon, position, key);
        }

        // TODO make this clever
        private static char RandomKey()
        {
            const string Keys = "abcdefghijklmnopqrstuvwxyz";

            return Keys[Random.Range(0, Keys.Length)];
        }
    }
}