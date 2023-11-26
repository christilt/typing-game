using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class PlayerTypingMovement : MonoBehaviour
{
    // TODO centralise Player
    [SerializeField] private GameObject _centre;

    private InputField _inputField;

    public event Action<KeyTile> OnCorrectKeyTyped;
    public event Action<KeyTile> OnIncorrectKeyTyped;

    public Vector2 Direction { get; private set; }

    public void EnableComponent()
    {
        _inputField = GetComponent<InputField>();
        _inputField.Select();
        _inputField.onValueChanged.AddListener(value =>
        {
            if (value == "")
                return;

            var key = value[0];
            if (TryMoveToKey(key, out var keyTile))
            {
                OnCorrectKeyTyped?.Invoke(keyTile);
            }
            else
            {
                OnIncorrectKeyTyped?.Invoke(keyTile);
            }

            _inputField.text = "";
        });
        _inputField.onEndEdit.AddListener(value =>
        {
            this.DoNextFrame(() => _inputField.Select());
        });
    }

    public void DisableComponent()
    {
        _inputField?.onValueChanged.RemoveAllListeners();
        _inputField?.onEndEdit.RemoveAllListeners();
    }

    private bool TryMoveToKey(char key, out KeyTile keyTile)
    {
        foreach (var candidateKeyTile in LevelTiles.Instance.GetNeighboursOf(transform.position))
        {
            if (candidateKeyTile.Key == key)
            {
                keyTile = candidateKeyTile;
                Direction = (Vector2)(candidateKeyTile.Position - transform.position).normalized;
                // TODO
                //_visual.transform.eulerAngles = GetEulerAnglesTowards(keyTile.Position);
                _centre.transform.eulerAngles = GetEulerAnglesTowards(candidateKeyTile.Position);
                transform.position = candidateKeyTile.Position;
                return true;
            }
        }
        keyTile = null;
        return false;
    }

    private Vector3 GetEulerAnglesTowards(Vector3 worldPosition)
    {
        var direction = (worldPosition -  transform.position).normalized;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return new Vector3(0, 0, angle);
    }

    private void OnDestroy()
    {
        DisableComponent();
    }
}