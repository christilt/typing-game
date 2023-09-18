using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInput input;

    [SerializeField] private Tilemap navigableTiles;
    [SerializeField] private Tilemap nonNavigableTiles;

    private void Awake()
    {
        input = new();
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void Start()
    {
        input.Main.Movement.performed += context => Move(context.ReadValue<Vector2>());
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
        var gridPosition = navigableTiles.WorldToCell(transform.position + (Vector3)direction);

        return navigableTiles.HasTile(gridPosition) 
            && !nonNavigableTiles.HasTile(gridPosition);
    }
}
