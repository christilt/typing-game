using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class PlayerTypingMovement : MonoBehaviour
{
    [SerializeField] private GameObject _visual;

    private InputField _inputField;

    public Vector2 Direction { get; private set; }

    public void EnableComponent()
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
            this.DoNextFrame(() => _inputField.Select());
        });
    }

    public void DisableComponent()
    {
        _inputField?.onValueChanged.RemoveAllListeners();
        _inputField?.onEndEdit.RemoveAllListeners();
    }

    private bool TryMoveToKey(char key)
    {
        foreach (var keyTile in LevelTiles.Instance.GetNeighboursOf(transform.position))
        {
            if (keyTile.Key == key)
            {
                Direction = (Vector2)(keyTile.Position - transform.position).normalized;
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
}