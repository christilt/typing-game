using System;

public class PlayerAttackManager : Singleton<PlayerAttackManager>
{
    public event Action<PlayerAttackReadiness> OnReadinessChanged;

    private bool IsReady => gameObject.activeInHierarchy && _readinessProportion == 1;
    private float _readinessProportion;
    public PlayerAttackReadiness Readiness => new PlayerAttackReadiness(_readinessProportion, IsReady);

    private void Start()
    {
        if (LevelSettingsManager.Instance.PlayerAttackSetting.Value == PlayerAttackSetting.None)
        {
            gameObject.SetActive(false);
            return;
        }

        if (LevelSettingsManager.Instance.PlayerAttackSetting.Value == PlayerAttackSetting.StartFull)
        {
            MaybeUpdateReadiness(1, forceEvent: true);
        }

        StatsManager.Instance.TypingRecorder.OnBurstNotification += NotifyOfStat;
        StatsManager.Instance.TypingRecorder.OnStreakNotification += NotifyOfStat;
    }

    private void OnDestroy()
    {
        if (StatsManager.Instance?.TypingRecorder != null)
        {
            StatsManager.Instance.TypingRecorder.OnBurstNotification -= NotifyOfStat;
            StatsManager.Instance.TypingRecorder.OnStreakNotification -= NotifyOfStat;
        }
    }

    public bool TryPlayerAttack()
    {
        if (!IsReady)
            return false;

        MaybeUpdateReadiness(0);

        return true;
    }

    public void NotifyOfStat(BurstStat stat)
    {
        if (stat.Category == StatCategory.Great)
        {
            MaybeIncreaseReadiness(0.4f);
        }
        else if (stat.Category == StatCategory.Good)
        {
            MaybeIncreaseReadiness(0.2f);
        }
    }

    public void NotifyOfStat(StreakStat stat)
    {
        if (stat.Category == StatCategory.Great)
        {
            MaybeIncreaseReadiness(0.4f);
        }
        else if (stat.Category == StatCategory.Good)
        {
            MaybeIncreaseReadiness(0.2f);
        }
    }

    private void MaybeIncreaseReadiness(float change) => MaybeUpdateReadiness(_readinessProportion + change);

    private void MaybeUpdateReadiness(float value, bool forceEvent = false)
    {
        var previousReadiness = _readinessProportion;

        _readinessProportion = Math.Clamp(value, 0, 1);
        var actualChange = _readinessProportion - previousReadiness;
        if (forceEvent || actualChange != 0)
        {
            OnReadinessChanged?.Invoke(Readiness);
        }
    }
}

public struct PlayerAttackReadiness
{
    public PlayerAttackReadiness(float proportion, bool isReady)
    {
        Proportion = proportion;
        IsReady = isReady;
    }

    public float Proportion { get; }
    public bool IsReady { get; }
    public float Percentage => Proportion * 100;
}