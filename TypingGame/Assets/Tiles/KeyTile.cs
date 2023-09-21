using UnityEngine;
using TMPro;

public class KeyTile
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