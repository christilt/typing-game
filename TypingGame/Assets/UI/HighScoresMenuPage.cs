using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class HighScoresMenuPage : MenuPage
{
    [SerializeField] private HighScoreRow[] _highScoreRows;

    private EditableHighScores _editableHighScores;

    protected override void OnEnable()
    {
        var levelId = LevelSettingsManager.Instance.LevelSettings.LevelId;
        var difficulty = GameSettingsManager.Instance.Difficulty;
        var stats = StatsManager.Instance.CalculateEndOfLevelStats();
        _editableHighScores ??= SaveDataManager.Instance.LoadHighScoresWithNewEntry(levelId, difficulty.Difficulty, stats);
        var whiteColourString = ColorUtility.ToHtmlStringRGB(Color.white);
        var lastPlayerIinitials = SaveDataManager.Instance.LoadLastPlayerInitials();

        for (var i = 0; i < _highScoreRows.Length; i++)
        {
            var highScoreRow = _highScoreRows[i];
            var isNew = _editableHighScores.LevelDifficultyNewEntryIndex == i;
            highScoreRow.Enable(isNew, i, _editableHighScores.LevelDifficultyHighScores, whiteColourString, lastPlayerIinitials);
        }

        base.OnEnable();
    }

    public void OnMenuHighScoreSubmit(TMP_InputField inputField)
    {
        var initials = inputField.text;
        if (initials.Length < 3)
            return;

        UpdatePlayerInitials(initials);

        var nextSelectable = _orderedSelectables.Skip(1).FirstOrDefault();
        nextSelectable?.Select();
    }

    // TODO: Not behaving as intended
    public void OnMenuHighScoreDeselect(TMP_InputField inputField)
    {
        var initials = inputField.text;
        if (initials.Length < 3)
        {
            inputField.Select();
            return;
        }

        UpdatePlayerInitials(initials);
    }

    private void UpdatePlayerInitials(string initials)
    {
        SaveDataManager.Instance.SaveLastPlayerInitials(initials);
        _editableHighScores.UpdateInitials(initials);
        SaveDataManager.Instance.Save(_editableHighScores.HighScores);
    }
}