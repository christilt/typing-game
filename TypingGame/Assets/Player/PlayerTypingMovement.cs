using System;
using System.Linq;
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
        // TODO Bug after a while returning false while still moving to the right key - maybe enabled x2?  Maybe fixed?
        _inputField.onValueChanged.RemoveAllListeners();
        _inputField.onValueChanged.AddListener(value =>
        {
            if (value == string.Empty)
                return;

            _inputField.text = string.Empty;

            var key = value[0];
            if (!IsTypingKey(key))
            {
                return;
            }

            if (TryMoveToKey(key, out var keyTile))
            {
                OnCorrectKeyTyped?.Invoke(keyTile);
            }
            else
            {
                OnIncorrectKeyTyped?.Invoke(keyTile);
            }
        });
        _inputField.onEndEdit.RemoveAllListeners();
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

    private bool IsTypingKey(char key)
    {
        var nonTypingKeys = new[] 
        {
            ' '  // Ignore spaces because they trigger attacks
        };
        return !nonTypingKeys.Contains(key);
    }

    private bool TryMoveToKey(char key, out KeyTile keyTile)
    {
        foreach (var candidateKeyTile in KeyTiles.Instance.GetNeighboursOf(transform.position))
        {
            if (candidateKeyTile.Key == key)
            {
                keyTile = candidateKeyTile;
                Direction = (Vector2)(candidateKeyTile.Position - transform.position).normalized;
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