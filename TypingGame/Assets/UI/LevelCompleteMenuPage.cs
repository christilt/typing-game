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
        var levelOrderValue = LevelSettingsManager.Instance.LevelSettings.NextLevel?.LevelOrder ?? LevelSettingsManager.Instance.LevelSettings.LevelOrder;
        var difficultyKey = GameSettingsManager.Instance.Difficulty.Difficulty.ToString();
        if (progress.DifficultyHighestReachedLevels.TryGetValue(difficultyKey, out var currentHighestReachedLevelOrderValue))
        {
            progress.DifficultyHighestReachedLevels[difficultyKey] = Math.Max(levelOrderValue, currentHighestReachedLevelOrderValue);
        }
        else
        {
            progress.DifficultyHighestReachedLevels.Add(difficultyKey, levelOrderValue);
        }

        SaveDataManager.Instance.Save(progress);

        base.OnEnable();
    }
}