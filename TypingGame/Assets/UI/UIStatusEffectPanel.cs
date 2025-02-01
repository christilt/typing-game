using System;
using System.Collections.Generic;
using UnityEngine;

public class UIStatusEffectPanel : MonoBehaviour
{
    [SerializeField] private UIStatusIcon _iconPrefab;
    [SerializeField] private Transform _firstIconPosition;
    [SerializeField] private float _xBetweenIcons;
    [SerializeField] private float _blinkIconsWhenBelowSeconds;

    private Dictionary<Type, UIStatusIcon> _iconsByType = new();

    public void RemoveEffect(CollectableEffectInfo info)
    {
        if (!_iconsByType.TryGetValue(info.Type, out var toRemove))
        {
            Debug.LogWarning($"Not found for removal in {nameof(_iconsByType)}: {info.Type}!");
            return;
        }

        _iconsByType.Remove(info.Type);
        GameObject.Destroy(toRemove.gameObject);

        // Update positions
        var i = 0;
        foreach(var typeIcon in _iconsByType)
        {
            typeIcon.Value.transform.position = PositionForIndex(i);
            i++;
        }
    }

    public void UpdateEffect(CollectableEffectInfo info)
    {
        if (info.DurationRemainingSeconds <= _blinkIconsWhenBelowSeconds)
        {
            if (!_iconsByType.TryGetValue(info.Type, out var toBlink))
            {
                Debug.LogWarning($"Not found for update in {nameof(_iconsByType)}: {info.Type}!");
                return;
            }

            toBlink.TryStartBlinking();
        }
    }

    public void AddEffect(CollectableEffectInfo info)
    {
        var nextIndex = _iconsByType.Count;
        var position = PositionForIndex(nextIndex);
        // TODO: Causing pauses?
        var added = UIStatusIcon.Instantiate(_iconPrefab, position, this.transform, info.Effect.Sprite);
        _iconsByType.Add(info.Type, added);
    }

    private Vector2 PositionForIndex(int index)
    {
        var rtlMultipler = -1;
        var offset = index * _xBetweenIcons * rtlMultipler;
        return new Vector3(offset + _firstIconPosition.position.x, _firstIconPosition.position.y);
    }
}