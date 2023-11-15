﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.tvOS;

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
        var added = new CollectableEffectInfo(effect.DurationSeconds, effect, typeof(T));
        var isReplacement = _currentEffectsByType.ContainsKey(typeof(T));
        if (isReplacement)
        {
            _currentEffectsByType.Remove(typeof(T));
        }

        _currentEffectsByType.Add(typeof(T), added);

        if (isReplacement)
        {
            OnCollectableEffectUpdate?.Invoke(added);
        }
        else
        {
            effect.ManagerApplyEffect();
            OnCollectableEffectAdded?.Invoke(added);
        }
    }

    private void Update()
    {
        foreach (var typeEffect in _currentEffectsByType)
        {
            typeEffect.Value.DurationRemainingSeconds -= Time.deltaTime;
            if (typeEffect.Value.DurationRemainingSeconds <= 0)
            {
                typeEffect.Value.Effect.ManagerRevertEffect();
                OnCollectableEffectRemoved?.Invoke(typeEffect.Value);
            }
            else if (typeEffect.Value.DurationRemainingSeconds <= (typeEffect.Value.DurationRemainingSecondsOfLastUpdate - _collectableEffectUpdateIntervalSeconds))
            {
                OnCollectableEffectUpdate?.Invoke(typeEffect.Value);
                typeEffect.Value.DurationRemainingSecondsOfLastUpdate = typeEffect.Value.DurationRemainingSeconds;
            }
        }

        _currentEffectsByType = _currentEffectsByType
            .Where(typeEffect => typeEffect.Value.DurationRemainingSeconds > 0)
            .ToDictionary(typeEffect => typeEffect.Key, typeEffect => typeEffect.Value);
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