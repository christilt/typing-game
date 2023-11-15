using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UIStatusEffectPanel : MonoBehaviour
{
    [SerializeField] private GameObject _iconPrefab;
    [SerializeField] private Transform _firstIconPosition;
    [SerializeField] private float _xBetweenIcons;

    private Dictionary<Type, GameObject> _iconsByType = new();

    private void Start()
    {
        CollectableEffectManager.Instance.OnCollectableEffectAdded += OnCollectableEffectAdded;
        CollectableEffectManager.Instance.OnCollectableEffectUpdate += OnCollectableEffectUpdate;
        CollectableEffectManager.Instance.OnCollectableEffectRemoved += OnCollectableEffectRemoved;
    }

    private void OnCollectableEffectRemoved(CollectableEffectInfo info)
    {
        var toRemove = _iconsByType[info.Type];
        if (toRemove == null)
            return;

        _iconsByType.Remove(info.Type);
        GameObject.Destroy(toRemove);

        // Update positions
        var i = 0;
        foreach(var typeIcon in _iconsByType)
        {
            typeIcon.Value.transform.position = PositionForIndex(i);
            i++;
        }
    }

    private void OnCollectableEffectUpdate(CollectableEffectInfo info)
    {
        // TODO - animation when 2s left
    }

    private void OnCollectableEffectAdded(CollectableEffectInfo info)
    {
        var nextIndex = _iconsByType.Count;
        _iconPrefab.SetActive(false);
        var added = GameObject.Instantiate(_iconPrefab, PositionForIndex(nextIndex), Quaternion.identity, this.transform);
        var image = added.GetComponent<Image>();
        image.sprite = info.Effect.Sprite;
        added.SetActive(true);
        _iconsByType.Add(info.Type, added);
    }

    private Vector2 PositionForIndex(int index)
    {
        var rtlMultipler = -1;
        var offset = index * _xBetweenIcons * rtlMultipler;
        return new Vector3(offset + _firstIconPosition.position.x, _firstIconPosition.position.y);
    }
}