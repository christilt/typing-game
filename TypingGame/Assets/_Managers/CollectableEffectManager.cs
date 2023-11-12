using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CollectableEffectManager : Singleton<CollectableEffectManager>
{
    private Dictionary<Type, CollectableEffectInfo> _currentEffectsByType = new();

    public void Apply(CollectableEffect effect)
    {
        effect.ApplyCollectableEffect();
    }

    public void ApplyStatus<T>(CollectableStatusEffect effect) where T : CollectableStatusEffect
    {
        if (_currentEffectsByType.ContainsKey(typeof(T)))
            _currentEffectsByType.Remove(typeof(T));

        _currentEffectsByType.Add(typeof(T), new CollectableEffectInfo(effect.DurationSeconds, effect));
        effect.ApplyCollectableEffect();
    }

    private void Update()
    {
        foreach(var typeEffect in _currentEffectsByType)
        {
            typeEffect.Value.DurationRemainingSeconds -= Time.deltaTime;
            if (typeEffect.Value.DurationRemainingSeconds <= 0)
            {
                typeEffect.Value.Effect.RevertCollectableEffect();
            }
        }

        _currentEffectsByType = _currentEffectsByType
            .Where(typeEffect => typeEffect.Value.DurationRemainingSeconds > 0)
            .ToDictionary(typeEffect => typeEffect.Key, typeEffect => typeEffect.Value);
    }
}

// TODO: Add sprite and name in SO for HUD
public class CollectableEffectInfo
{
    public CollectableEffectInfo(float durationRemainingSeconds, CollectableStatusEffect effect)
    {
        DurationRemainingSeconds = durationRemainingSeconds;
        Effect = effect;
    }

    public CollectableStatusEffect Effect { get; }

    public float DurationRemainingSeconds { get; set; }
}