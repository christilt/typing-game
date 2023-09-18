using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class KeyMap : MonoBehaviour
{
    [SerializeField] private Tilemap navigableTiles;

    private Dictionary<Vector3Int, char> _keys = new();

    //private void Awake()
    //{
    //    foreach(var tile in navigableTiles.)
    //}
}
