﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HighScoresMenuPage : MenuPage
{
    [SerializeField] private HighScoreRow[] _highScoreRows;
    [SerializeField] private Button _nextButton;

    private EditableHighScores _editableHighScores;

    protected override void OnEnable()
    {
        var levelId = LevelSettingsManager.Instance.LevelSettings.LevelId;
        var difficulty = GameSettingsManager.Instance.Difficulty;
        var stats = StatsManager.Instance.CalculateEndOfLevelStats();
        _editableHighScores ??= SaveDataManager.Instance.LoadHighScoresWithNewEntry(levelId, difficulty.Difficulty, stats);
        var whiteColourString = ColorUtility.ToHtmlStringRGBA(Color.white);
        var dimWhite = Color.white;
        dimWhite.a = 128;
        var dimWhiteColourString = ColorUtility.ToHtmlStringRGB(dimWhite);
        var lastPlayerIinitials = SaveDataManager.Instance.LoadLastPlayerInitials();

        for (var i = 0; i < _highScoreRows.Length; i++)
        {
            var highScoreRow = _highScoreRows[i];
            var isNew = _editableHighScores.LevelDifficultyNewEntryIndex == i;
            highScoreRow.Enable(isNew, i, _editableHighScores.LevelDifficultyHighScores, whiteColourString, dimWhiteColourString, lastPlayerIinitials);
        }

        base.OnEnable();
    }

    public void OnMenuHighScoreSubmit(TMP_InputField inputField)
    {
        var initials = inputField.text;
        if (initials.Length < 3)
        {
            _nextButton.interactable = false;
            inputField.
            StartCoroutine(SelectOnNextFrame(inputField)); // Selecting same frame doesn't work.  Select so that user remains editing text
            return;
        }

        UpdatePlayerInitials(initials);

        _nextButton.interactable = true;

        var nextSelectable = _orderedSelectables.Skip(1).FirstOrDefault();
        nextSelectable?.Select();
    }
    private IEnumerator SelectOnNextFrame(TMP_InputField inputField)
    {
        yield return new WaitForEndOfFrame();
        inputField.Select();
        inputField.ActivateInputField();
    }

    private void UpdatePlayerInitials(string initials)
    {
        SaveDataManager.Instance.SaveLastPlayerInitials(initials);
        _editableHighScores.UpdateInitials(initials);
        SaveDataManager.Instance.Save(_editableHighScores.HighScores);
    }
}