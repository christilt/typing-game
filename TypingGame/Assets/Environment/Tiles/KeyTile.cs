using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class KeyTile
{
    private KeyTile(Vector3Int position, char key)
    {
        Key = key;
        Position = position;
    }

    public char Key { get; }
    public Vector3Int Position { get; }

    public static KeyTile Instantiate(GameObject iconPrefab, Vector3Int position, GameObject parent, IEnumerable<char> deniedKeys)
    {
        var key = RandomKey(deniedKeys);

        var icon = Object.Instantiate(original: iconPrefab, position: position, rotation: Quaternion.identity, parent: parent.transform);
        var iconText = icon.GetComponentInChildren<TextMeshProUGUI>();
        iconText.text = $"{key}";

        return new KeyTile(position, key);
    }

    // TODO make this clever
    private static char RandomKey(IEnumerable<char> deniedKeys)
    {
        const string Keys = "abcdefghijklmnopqrstuvwxyz";
        var allowedKeys = Keys.Except(deniedKeys).ToArray();

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