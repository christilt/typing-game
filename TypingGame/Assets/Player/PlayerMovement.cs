using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class PlayerMovement : MonoBehaviour
{
    private PlayerInput _input;

    private InputField _inputField;

    private void Awake()
    {
        _input = new();
    }

    private void OnEnable()
    {
        _inputField = GetComponent<InputField>();
        _inputField.Select();
        _inputField.onValueChanged.AddListener(value =>
        {
            if (value == "")
                return;

            TryMoveToKey(value[0]);

            _inputField.text = "";
        });
        _inputField.onEndEdit.AddListener(value =>
        {
            StartCoroutine(DoNextFrame(() => _inputField.Select()));
        });
        _input.Enable();
    }

    private void OnDisable()
    {
        _input.Disable();
    }

    private bool TryMoveToKey(char key)
    {
        foreach(var keyTile in LevelTiles.Instance.GetNeighboursOf(transform.position))
        {
            if (keyTile.Key == key)
            {
                transform.position = keyTile.Position;
                return true;
            }
        }
        return false;
    }

    private IEnumerator DoNextFrame(Action action)
    {
        yield return null;

        action();
    }
}
