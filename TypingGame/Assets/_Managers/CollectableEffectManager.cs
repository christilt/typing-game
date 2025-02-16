using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CollectableEffectManager : Singleton<CollectableEffectManager>
{
    [SerializeField] private float _collectableEffectUpdateIntervalSeconds;

    private Dictionary<Type, CollectableEffectInfo> _currentEffectsByType = new();

    public event Action<CollectableEffectInfo> OnCollectableEffectAdded;
    public event Action<CollectableEffectInfo> OnCollectableEffectUpdate;
    public event Action<CollectableEffectInfo> OnCollectableEffectRemoved;

    public void Apply(CollectableEffect effect)
    {
        effect.ManagerApplyEffect();
    }

    public void ApplyStatus<T>(CollectableStatusEffect effect) where T : CollectableStatusEffect
    {
        Debug.Log($"Collectable effect applied: {effect.name}");

        var added = new CollectableEffectInfo(effect.DurationSeconds, effect, typeof(T));
        var isReplacement = _currentEffectsByType.TryGetValue(typeof(T), out var replaced);
        if (isReplacement)
        {
            _currentEffectsByType.Remove(typeof(T));
            OnCollectableEffectRemoved?.Invoke(replaced);
        }

        _currentEffectsByType.Add(typeof(T), added);

        effect.ManagerApplyEffect();
        OnCollectableEffectAdded?.Invoke(added);
    }

    private void Update()
    {
        List<Type> typesToRemove = null;
        foreach (var typeEffect in _currentEffectsByType)
        {
            var effect = typeEffect.Value;
            effect.DurationRemainingSeconds -= Time.deltaTime;
            if (effect.DurationRemainingSeconds <= 0)
            {
                effect.Effect.ManagerRevertEffect();
                OnCollectableEffectRemoved?.Invoke(effect);
                typesToRemove ??= new();
                typesToRemove.Add(typeEffect.Key);
            }
            else if (effect.DurationRemainingSeconds <= (effect.DurationRemainingSecondsOfLastUpdate - _collectableEffectUpdateIntervalSeconds))
            {
                OnCollectableEffectUpdate?.Invoke(effect);
                effect.DurationRemainingSecondsOfLastUpdate = effect.DurationRemainingSeconds;
            }
        }

        if (typesToRemove != null)
        {
            foreach (var type in typesToRemove)
            {
                _currentEffectsByType.Remove(type);
            }
        }
    }
}

public class CollectableEffectInfo
{
    public CollectableEffectInfo(float durationRemainingSeconds, CollectableStatusEffect effect, Type type)
    {
        DurationRemainingSeconds = durationRemainingSeconds;
        DurationRemainingSecondsOfLastUpdate = durationRemainingSeconds;
        Effect = effect;
        Type = type;
    }

    public CollectableStatusEffect Effect { get; }
    public Type Type { get; }

    public float DurationRemainingSeconds { get; set; }
    public float DurationRemainingSecondsOfLastUpdate { get; set; }
}