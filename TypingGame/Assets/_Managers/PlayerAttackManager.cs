using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerAttackManager : Singleton<PlayerAttackManager>
{
    public event Action<float> OnReadinessChanged;

    public float ReadinessProportion { get; private set; }

    public bool IsReady => gameObject.activeInHierarchy && ReadinessProportion == 1;

    private void Start()
    {
        if (SettingsManager.Instance.PlayerAttackSetting.Value == PlayerAttackSetting.None)
        {
            gameObject.SetActive(false);
            return;
        }

        if (SettingsManager.Instance.PlayerAttackSetting.Value == PlayerAttackSetting.StartFull)
        {
            MaybeUpdateReadiness(1, forceEvent: true);
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

    private void MaybeIncreaseReadiness(float change) => MaybeUpdateReadiness(ReadinessProportion + change);

    private void MaybeUpdateReadiness(float value, bool forceEvent = false)
    {
        var previousReadiness = ReadinessProportion;

        ReadinessProportion = Math.Clamp(value, 0, 1);
        // TODO remove
        Debug.Log($"Player attack readiness={ReadinessProportion}; IsReady={IsReady}");
        var actualChange = ReadinessProportion - previousReadiness;
        if (forceEvent || actualChange != 0)
        {
            OnReadinessChanged?.Invoke(actualChange);
        }
    }
}