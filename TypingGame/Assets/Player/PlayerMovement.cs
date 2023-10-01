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
    [SerializeField] private GameObject _visual;

    private InputField _inputField;

    private void Start()
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
    }

    private bool TryMoveToKey(char key)
    {
        foreach (var keyTile in LevelTiles.Instance.GetNeighboursOf(transform.position))
        {
            if (keyTile.Key == key)
            {
                _visual.transform.eulerAngles = GetEulerAnglesTowards(keyTile.Position);
                transform.position = keyTile.Position;
                return true;
            }
        }
        return false;
    }

    private Vector3 GetEulerAnglesTowards(Vector3 worldPosition)
    {
        var direction = (worldPosition -  transform.position).normalized;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return new Vector3(0, 0, angle);
    }

    private IEnumerator DoNextFrame(Action action)
    {
        yield return null;

        action();
    }
}