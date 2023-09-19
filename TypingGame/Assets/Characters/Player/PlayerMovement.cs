using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInput _input;

    [SerializeField] private Tilemap _pathTiles;

    private void Awake()
    {
        _input = new();
    }

    private void OnEnable()
    {
        _input.Enable();
    }

    private void OnDisable()
    {
        _input.Disable();
    }

    private void Start()
    {
        _input.Main.Movement.performed += context => Move(context.ReadValue<Vector2>());
    }

    private void Move(Vector2 direction)
    {
        if (CanMove(direction))
        {
            transform.position += (Vector3)direction;
        }
    }

    private bool CanMove(Vector2 direction)
    {
        var gridPosition = _pathTiles.WorldToCell(transform.position + (Vector3)direction);

        return _pathTiles.HasTile(gridPosition);
    }
}
