using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class KeyTile
{
    private KeyTile(Vector3Int position, Vector3 centre, char key)
    {
        Key = key;
        Position = position;
        Centre = centre;
    }

    public char Key { get; }
    public Vector3Int Position { get; }
    public Vector3 Centre { get; }

    public static KeyTile Instantiate(GameObject iconPrefab, Vector3Int position, Vector3 centre, GameObject parent, IEnumerable<char> deniedKeys)
    {
        // TODO remove
        var allowedKeys = SettingsManager.Instance.CharacterSet.Value.Characters.Except(deniedKeys).ToArray();
        Debug.Log($"Instantiating key at {position} based on allowed keys of {string.Join(", ", allowedKeys)}");

        var key = RandomKey(deniedKeys);

        var icon = Object.Instantiate(original: iconPrefab, position: position, rotation: Quaternion.identity, parent: parent.transform);
        var iconText = icon.GetComponentInChildren<TextMeshProUGUI>();
        iconText.text = $"{key}";

        return new KeyTile(position, centre, key);
    }

    private static char RandomKey(IEnumerable<char> deniedKeys)
    {
        var keys = SettingsManager.Instance.CharacterSet.Value.Characters;
        var allowedKeys = keys.Except(deniedKeys).ToArray();

        return allowedKeys[Random.Range(0, allowedKeys.Length)];
    }

    public override bool Equals(object obj)
    {
        return obj is KeyTile tile &&
               Key == tile.Key &&
               Position.Equals(tile.Position);
    }

    public override int GetHashCode()
    {
        return System.HashCode.Combine(Key, Position);
    }
}