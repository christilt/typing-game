using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LevelCompleteMenuPage : MenuPage
{
    protected override void OnEnable()
    {
        var progress = SaveDataManager.Instance.LoadGameProgress();
        var levelOrderValue = LevelSettingsManager.Instance.LevelSettings.LevelOrder;
        var difficultyKey = GameSettingsManager.Instance.Difficulty.Difficulty.ToString();
        if (progress.DifficultyHighestCompleteLevels.TryGetValue(difficultyKey, out var currentHighestCompleteLevelOrderValue))
        {
            progress.DifficultyHighestCompleteLevels[difficultyKey] = Math.Max(levelOrderValue, currentHighestCompleteLevelOrderValue);
        }
        else
        {
            progress.DifficultyHighestCompleteLevels.Add(difficultyKey, levelOrderValue);
        }

        SaveDataManager.Instance.Save(progress);

        base.OnEnable();
    }
}